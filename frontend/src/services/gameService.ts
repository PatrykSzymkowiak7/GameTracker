
import type { Game, PagedResult, CreateGameRequest, UpdateGameRequest, Developer, GamePlatform, Genre } from "../types/game";
import { apiRequest } from "../api/apiClient";

export function getGames(token: string): Promise<PagedResult<Game>>{
    return apiRequest<PagedResult<Game>>("/api/v1/Games", token)
}

export function createGame(token: string, request: CreateGameRequest) : Promise<Game>{
    return apiRequest<Game>("/api/v1/Games", token, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
    })
}

export function deleteGame(token: string, id: number) : Promise<void>{
    return apiRequest<void>("/api/v1/Games/" + id, token, {
        method: "DELETE"
    })
}

export function updateGame(token: string, id: number, request: UpdateGameRequest) : Promise<Game>{
    return apiRequest<Game>(`/api/v1/Games/${id}`, token, {
        headers: {
            "Content-Type":"application/json"
        },
        method: "PUT",
        body: JSON.stringify(request)
    });
}

export function getDevelopers(token: string) : Promise<Developer[]>{
    return apiRequest<Developer[]>("/api/v1/Developers", token);
}

export function getGenres(token: string) : Promise<Genre[]>{
    return apiRequest<Genre[]>("/api/v1/Genres", token);
}

export function getPlatforms(token: string) : Promise<GamePlatform[]>{
    return apiRequest<GamePlatform[]>("/api/v1/Platforms", token);
}