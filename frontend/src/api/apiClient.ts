import { UnauthorizedError } from "../errors/UnauthorizedError";

export async function apiRequest<T>(
    endpoint: string,
    token: string
): Promise<T>{
    const response = await fetch("https://localhost:7089" + endpoint, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })

    if(response.status === 401){
        throw new UnauthorizedError();
    }

    if(!response.ok){
        throw new Error("Something went wrong");
    }

    const data = await response.json();

    return data;
}