import { UnauthorizedError } from "../errors/UnauthorizedError";
import type { Game, PagedResult } from "../types/game";

async function getGames(token: string): Promise<PagedResult<Game>> {
    const response = await fetch("https://localhost:7089/api/v1/Games", {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (response.status === 401) {
        throw new UnauthorizedError();
    }

    if (!response.ok) {
        throw new Error("Failed to fetch games");
    }

    const data: PagedResult<Game> = await response.json();

    return data;
}

export { getGames };