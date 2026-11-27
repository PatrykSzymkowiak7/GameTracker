import GameCard from "../components/GameCard";
import { type Game, type GamePlatform, type Developer, 
    type Genre, type GameStatus, GAME_STATUSES, type CreateGameRequest } from "../types/game";
import { useState, useEffect } from "react";
import { getGames, deleteGame, getDevelopers, getGenres, getPlatforms, createGame } from "../services/gameService";
import { useAuth } from "../context/AuthContext"; 
import { UnauthorizedError } from "../errors/UnauthorizedError";

function GamesPage() {
    const [games, setGames] = useState<Game[]>([]);
    const [newTitle, setNewTitle] = useState("");
    const [newHours, setNewHours] = useState(0);
    const [selectedGenreIds, setSelectedGenreIds] = useState<number[]>([]);
    const [selectedPlatformIds, setSelectedPlatformIds] = useState<number[]>([]);
    const [selectedGameStatus, setSelectedGameStatus] = useState<GameStatus>("Backlog");
    const [rating, setRating] = useState<number | null>(null);
    const [developerId, setDeveloperId] = useState<string>("");
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { token, logout } = useAuth();
    const [editingGame, setEditingGame] = useState<Game | null>(null);
    const [editTitle, setEditingTitle] = useState("");
    const [editHours, setEditHours] = useState(0);
    const [developers, setDevelopers] = useState<Developer[]>([]);
    const [genres, setGenres] = useState<Genre[]>([]);
    const [platforms, setPlatforms] = useState<GamePlatform[]>([]);
    const [isAddFormVisible, setIsAddFormVisible] = useState(false);

    const totalHours = games.reduce(
        (sum, game) => sum + game.hoursPlayed,
        0
    );

    function handlePlatformChange(platformId: number){
        setSelectedPlatformIds(currentIds => {
            if(currentIds.includes(platformId)){
                return currentIds.filter(id => id !== platformId);
            }

            return [...currentIds, platformId];
        })
    }

    function handleGenreChange(genreId: number){
        setSelectedGenreIds(currentIds => {
            if(currentIds.includes(genreId)){
                return currentIds.filter(id => id !== genreId)
            }

            return[...currentIds, genreId];
        });
    };
    
    function handleEdit(game: Game){
        setEditingGame(game);
        setEditingTitle(game.title);
        setEditHours(game.hoursPlayed);
    }

    async function handleAddGame() {
        setError("");

        if(!token){
            setError("Token is null");
            return;
        }

        if (!newTitle.trim()) {
            setError("Title is empty");
            return;
        }

        if(selectedPlatformIds.length === 0){
            setError("Select platforms");
            return;
        }
        
        if(selectedGenreIds.length === 0){
            setError("Select genres");
            return;
        }        
        
        if(developerId === ""){
            setError("Select developer");
            return;
        }

        const request: CreateGameRequest = {
            title: newTitle,
            developerId: Number(developerId),
            genreIds: selectedGenreIds,
            platformIds: selectedPlatformIds,
            status: selectedGameStatus,
            rating: rating,
            hoursPlayed: newHours
        }
        
        try{
            setIsLoading(true);

            const response = await createGame(token, request);

            setGames(currentGames => [...currentGames, response]);

            setNewTitle("");
            setNewHours(0);
            setDeveloperId("");
            setSelectedGenreIds([]);
            setSelectedPlatformIds([]);
            setRating(null);
            setIsAddFormVisible(false);
        }
        catch(error){
            if(error instanceof UnauthorizedError){
                logout();
                return;
            }

            if(error instanceof Error){
                setError(error.message);
            }
        }
        finally{
            setIsLoading(false);
        }
    }

    async function handleDeleteGame(id: number){
        setError(null);

        if(!token){
            setError("Token is null");
            return;
        }

        if(id <= 0){
            setError("Id is not valid")
            return;
        }

        try{
            setIsLoading(true);
            await deleteGame(token, id);
            setGames(currentGames => currentGames.filter(g => g.id !== id));
        }
        catch(error){
            if(error instanceof UnauthorizedError){
                logout();
                return;
            }

            if(error instanceof Error){
                setError(error.message);
            }
        }
        finally{
            setIsLoading(false);
        }
    }

    useEffect(() => {
        async function loadGames() {
            try {
                setIsLoading(true);
                setError(null);

                if(!token){
                    return;
                }
                
                const [data, developers, genres, platforms] = await Promise.all([
                    getGames(token),
                    getDevelopers(token),
                    getGenres(token),
                    getPlatforms(token)
                ]);

                setDevelopers(developers);
                setPlatforms(platforms);
                setGenres(genres);
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

        
        {isAddFormVisible && (
            <form
            onSubmit={(event) => {
                event.preventDefault();
                handleAddGame();
            }}
            className="mb-8 rounded-xl border border-slate-700 bg-slate-800 p-6"
            >
                <div className="mb-8">
                    <h2 className="mb-4 text-lg font-semibold text-white">
                        Game information
                    </h2>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div className="flex flex-col gap-2">
                            <label htmlFor="title" className="text-sm text-gray-400">
                                Title
                            </label>
                            <input
                                id="title"
                                value={newTitle}
                                onChange={(event) => setNewTitle(event.target.value)}
                                placeholder="Game title"
                                className="w-full rounded-lg border border-slate-700 bg-slate-900 p-3 text-white focus:border-blue-500 focus:outline-none"
                            />
                        </div>

                        <div className="flex flex-col gap-2">
                            <label htmlFor="developer" className="text-sm text-gray-400">
                                Developer
                            </label>
                            <select
                                id="developer"
                                value={developerId}
                                onChange={(event) => setDeveloperId(event.target.value)}
                                className="w-full rounded-lg border border-slate-700 bg-slate-900 p-3 text-white focus:border-blue-500 focus:outline-none"
                            >
                                <option value="">Select developer</option>
                                {developers.map(developer => (
                                    <option key={developer.id} value={developer.id}>
                                        {developer.name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="flex flex-col gap-2">
                            <label htmlFor="gameStatus" className="text-sm text-gray-400">
                                Game status
                            </label>
                            <select
                                id="gameStatus"
                                value={selectedGameStatus}
                                onChange={(event) => setSelectedGameStatus(event.target.value as GameStatus)}
                                className="w-full rounded-lg border border-slate-700 bg-slate-900 p-3 text-white focus:border-blue-500 focus:outline-none"
                            >
                                {GAME_STATUSES.map(status => (
                                    <option key={status} value={status}>
                                        {status}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                </div>

                <div className="mb-8">
                    <h2 className="mb-4 text-lg font-semibold text-white">
                        Categories
                    </h2>

                    <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
                        <div>
                            <p className="mb-3 text-sm text-gray-400">Genres</p>
                            <div className="flex flex-wrap gap-3">
                                {genres.map(genre => (
                                    <label
                                        key={genre.id}
                                        className="flex cursor-pointer items-center gap-2 rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm text-gray-300 hover:border-slate-500"
                                    >
                                        <input
                                            type="checkbox"
                                            value={genre.id}
                                            checked={selectedGenreIds.includes(genre.id)}
                                            onChange={() => handleGenreChange(genre.id)}
                                            className="accent-blue-500"
                                        />
                                        {genre.name}
                                    </label>
                                ))}
                            </div>
                        </div>

                        <div>
                            <p className="mb-3 text-sm text-gray-400">Platforms</p>
                            <div className="flex flex-wrap gap-3">
                                {platforms.map(platform => (
                                    <label
                                        key={platform.id}
                                        className="flex cursor-pointer items-center gap-2 rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm text-gray-300 hover:border-slate-500"
                                    >
                                        <input
                                            type="checkbox"
                                            value={platform.id}
                                            checked={selectedPlatformIds.includes(platform.id)}
                                            onChange={() => handlePlatformChange(platform.id)}
                                            className="accent-blue-500"
                                        />
                                        {platform.name}
                                    </label>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>

                <div>
                    <h2 className="mb-4 text-lg font-semibold text-white">
                        Playtime & rating
                    </h2>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div className="flex flex-col gap-2">
                            <label htmlFor="hoursPlayed" className="text-sm text-gray-400">
                                Hours played
                            </label>
                            <input
                                id="hoursPlayed"
                                type="number"
                                min="0"
                                value={newHours}
                                onChange={(event) => setNewHours(event.target.valueAsNumber)}
                                className="w-full rounded-lg border border-slate-700 bg-slate-900 p-3 text-white focus:border-blue-500 focus:outline-none"
                            />
                        </div>

                        <div className="flex flex-col gap-2">
                            <label htmlFor="rating" className="text-sm text-gray-400">
                                Rating (0-10)
                            </label>
                            <input
                                id="rating"
                                type="number"
                                min="0"
                                max="10"
                                step="0.5"
                                value={rating ?? ""}
                                onChange={(event) => {
                                    const value = event.target.valueAsNumber;
                                    setRating(Number.isNaN(value) ? null : value);
                                }}
                                className="w-full rounded-lg border border-slate-700 bg-slate-900 p-3 text-white focus:border-blue-500 focus:outline-none"
                            />
                        </div>
                    </div>

                    <div className="mt-6 flex justify-end">
                        <button
                            type="submit"
                            disabled={isLoading}
                            className="rounded-lg bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                            {isLoading ? "Saving..." : "Add game"}
                        </button>

                        <button
                            type="button"
                            onClick={() => setIsAddFormVisible(false)}
                            className="rounded-lg border border-slate-600 px-4 py-2 text-slate-300 hover:bg-slate-700"
                        >
                            Cancel
                        </button>
                    </div>
                </div>
            </form>
        )}

        <button
            type="button"
            onClick={() => setIsAddFormVisible(true)}
            className="mb-6 rounded-lg bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
        >
                + Add game
        </button>


            {editingGame && (
                <form
                onSubmit={(event) => {
                    event.preventDefault();
                }}
                className="mb-8 flex flex-wrap gap-3"
                >

                    <input
                        value={editTitle}
                        onChange={(event) => setEditingTitle(event.target.value)}
                        placeholder="Game title"
                        className="rounded-lg border border-slate-700 bg-slate-800 p-3 text-white"
                    />

                    <input
                        type="number"
                        min="0"
                        value={editHours}
                        onChange={(event) => setEditHours(event.target.valueAsNumber)}
                        className="w-32 rounded-lg border border-slate-700 bg-slate-800 p-3 text-white"
                    />

                    <button
                        type="button"
                        onClick={() => setEditingGame(null)}
                        className="rounded-lg bg-slate-600 px-5 py-3 font-semibold hover:bg-slate-700"
                    >
                        Cancel
                    </button>

                    <button
                        type="submit"
                        className="rounded-lg bg-blue-600 px-5 py-3 font-semibold hover:bg-blue-700"
                    >
                        Save
                    </button>
                 </form>
            )}

            <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
                {isLoading && <p>Loading games...</p>}

                {error && <p className="text-red-500">Error: {error}</p>}

                {!error && !isLoading && (
                    games.map(game => (
                        <GameCard key={game.id} game={game} onEdit={(selectedGame) => handleEdit(selectedGame)} />
                    )))}
            </div>
        </div>
    );
}

export default GamesPage;