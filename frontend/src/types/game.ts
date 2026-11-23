export interface Game {
    id: number;
    title: string;
    hoursPlayed: number;
    status: "Playing" | "Completed" | "Backlog" | "Dropped";
    imageUrl: string;
}

export interface PagedResult<T>{
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}