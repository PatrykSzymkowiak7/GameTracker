import GameCard from "../components/GameCard";
import type { Game } from "../types/game";
import { useState, useEffect } from "react";
import { getGames } from "../services/gameService";
import { useAuth } from "../context/AuthContext"; 
import { UnauthorizedError } from "../errors/UnauthorizedError";

function GamesPage() {
    const [games, setGames] = useState<Game[]>([]);
    const [newTitle, setNewTitle] = useState("");
    const [newHours, setNewHours] = useState(0);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { token, logout } = useAuth();

    const totalHours = games.reduce(
        (sum, game) => sum + game.hoursPlayed,
        0
    );

    function handleAddGame() {
        if (!newTitle.trim()) {
            return;
        }

        const newGame: Game = {
            id: Date.now(),
            title: newTitle,
            hoursPlayed: Number(newHours),
            status: "Backlog",
            imageUrl: ""
        }

        setGames(currentGames => [...currentGames, newGame]);
        setNewTitle("");
        setNewHours(0);
    }

    useEffect(() => {
        async function loadGames() {
            try {
                setIsLoading(true);
                setError(null);

                if(!token){
                    return;
                }

                const data = await getGames(token);

                setGames(data.items);
            }
            catch (error) {
                if (error instanceof Error) {
                    if(error instanceof UnauthorizedError){
                        logout();
                        return;
                    }

                    setError(error.message);
                    console.error("Error loading games:", error.message);
                }
            }
            finally {
                setIsLoading(false);
            }
        }

        loadGames();
    }, [token, logout]);

    return (
        <div>
            <h1 className="mb-2 text-3xl font-bold">
                My games
            </h1>

            <p className="mb-8 text-gray-400">
                Manage your game collection.
            </p>

            <div className="mb-8 grid grid-cols-1 gap-4 md:grid-cols-3">
                <div className="rounded-xl bg-slate-800 p-5">
                    <p className="text-sm text-gray-400">All games</p>
                    <p className="mt-2 text-3xl font-bold">{games.length}</p>
                </div>

                <div className="rounded-xl bg-slate-800 p-5">
                    <p className="text-sm text-gray-400">Total hours played</p>
                    <p className="mt-2 text-3xl font-bold">{totalHours}</p>
                </div>

                <div className="rounded-xl bg-slate-800 p-5">
                    <p className="text-sm text-gray-400">Completed games</p>
                    <p className="mt-2 text-3xl font-bold">
                        {games.filter(game => game.status === "Completed").length}
                    </p>
                </div>
            </div>

            <form
                onSubmit={(event) => {
                    event.preventDefault();
                    handleAddGame();
                }}
                className="mb-8 flex flex-wrap gap-3"
            >

                <input
                    value={newTitle}
                    onChange={(event) => setNewTitle(event.target.value)}
                    placeholder="Game title"
                    className="rounded-lg border border-slate-700 bg-slate-800 p-3 text-white"
                />

                <input
                    type="number"
                    min="0"
                    value={newHours}
                    onChange={(event) => setNewHours(event.target.valueAsNumber)}
                    className="w-32 rounded-lg border border-slate-700 bg-slate-800 p-3 text-white"
                />

                <button
                    type="submit"
                    className="rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-700">
                    Add game
                </button>
            </form>

            <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
                {isLoading && <p>Loading games...</p>}

                {error && <p className="text-red-500">Error: {error}</p>}

                {!error && !isLoading && (
                    games.map(game => (
                        <GameCard key={game.id} game={game} />
                    )))}
            </div>
        </div>
    );
}

export default GamesPage;