import type { ApiProblemDetails, AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";

const API_AUTH_URL = "https://localhost:7089/api/auth";

async function login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await postJson(
        `${API_AUTH_URL}/login`, 
        credentials, 
        "Login failed");

    return await response.json();
};

async function register(credentials: RegisterRequest): Promise<void> {
    const response = await postJson(`${API_AUTH_URL}/register`,
        credentials,
        "Registration failed");

    return response.json();
}

async function postJson(url: string, body: unknown, fallbackMessage: string) : Promise<Response>{
    let response: Response;

    try{
        response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(body)
        });
    }
    catch{
        throw new Error("Cannot connect to the server. Please try again later.")
    }
    
    if(!response.ok){
        let message = fallbackMessage;

        try{
            const problem: ApiProblemDetails = await response.json();
            message = problem.title ?? fallbackMessage;
        }
        catch{
            // If no message is found in problem details we return the fallback message
        }

        throw new Error(message);
    }

    return response;
}

export { login, register };