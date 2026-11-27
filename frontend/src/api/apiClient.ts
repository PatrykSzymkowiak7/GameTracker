import { UnauthorizedError } from "../errors/UnauthorizedError";

export async function apiRequest<T>(
    endpoint: string,
    token: string,
    options?: RequestInit
): Promise<T>{
    const response = await fetch("https://localhost:7089" + endpoint, 
        {
            ...options,
            headers: { 
                ...options?.headers, 
                "Authorization": `Bearer ${token}` 
            }
        }
    )

    if(response.status === 401){
        throw new UnauthorizedError();
    }

    if(!response.ok){
        throw new Error("Something went wrong");
    }

    if(response.status === 204){
        return undefined as T;
    }

    const data = await response.json();

    return data;
}