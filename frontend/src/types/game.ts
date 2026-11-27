export interface Game {
    id: number;
    title: string;
    status: GameStatus;
    rating: number | null;
    hoursPlayed: number;
    developer: Developer | null;
    genres: Genre[];
    platforms: GamePlatform[];
    imageUrl: string;
}

export interface PagedResult<T>{
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export interface CreateGameRequest {
    title: string;
    developerId: number;
    genreIds: number[];
    platformIds: number[];
    status: GameStatus;
    rating: number | null;
    hoursPlayed: number;
}

export type GameStatus = "Backlog" | "Playing" | "Completed" | "Dropped";

export const GAME_STATUSES: GameStatus[] = [
    "Backlog",
    "Playing",
    "Completed",
    "Dropped"
];

export interface UpdateGameRequest {
    title: string;
    developerId: number;
    genreIds: number[];
    platformIds: number[];
    status: GameStatus;
    rating: number | null;
    hoursPlayed: number;
}

export interface GamePlatform{
    id: number;
    name: string;
}
export interface Developer{
    id: number;
    name: string;
}
export interface Genre{
    id: number;
    name: string;
}