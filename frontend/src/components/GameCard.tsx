import type { Game } from "../types/game";

interface GameCardProps {
    game: Game;
    onEdit: (game: Game) => void;
}

function GameCard({ game, onEdit }: GameCardProps) {
    return (
        <div className="overflow-hidden rounded-xl border border-slate-700 bg-slate-800 transition hover:border-blue-500">
            {game.imageUrl ? (<img
                src={game.imageUrl}
                alt={game.title}
                className="h-40 w-full object-cover"
            />) : (
                <div className="flex h-40 items-center justify-center bg-slate-700 text-gray-400">
                    No cover
                </div>
            )
            }

            <div className="p-5">
                <h2 className="mb-2 text-xl font-semibold">
                    {game.title}
                </h2>

                <p className="mb-3 text-sm text-gray-400">
                    Hours played: {game.hoursPlayed}
                </p>

                <div className="mt-4 flex items-center justify-between">
                    <span className="rounded-full bg-blue-500/20 px-3 py-1 text-xs text-blue-300">
                        {game.status}
                    </span>

                    <button
                        type="button"
                        onClick={() => onEdit(game)}
                        className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium transition hover:bg-blue-500"
                    >
                        Edit
                    </button>
                </div>
            </div>
        </div>
    );
}

export default GameCard;