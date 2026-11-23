import type { ApiProblemDetails, AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";

async function login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await fetch("https://localhost:7089/api/auth/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    });

    if (!response.ok) {
        const problem: ApiProblemDetails = await response.json();

        throw new Error(problem.title ?? "Login failed");
    }

    const data: AuthResponse = await response.json();

    return data;
};

async function register(credentials: RegisterRequest): Promise<void> {
    const response = await fetch("https://localhost:7089/api/auth/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    });

    if(!response.ok){
        const problem: ApiProblemDetails = await response.json();

        throw new Error(problem.title ?? "Registration failed");
    }
}

export { login, register };