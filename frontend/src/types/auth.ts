export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
    expiresAtUtc: string;
}

export interface AuthContextType {
    token: string | null;
    setToken: (token: string | null) => void;
    logout: () => void;
}

export interface RegisterRequest{
    username: string;
    email: string;
    password: string;
}

export interface ApiProblemDetails{
    title?: string;
    status?: number;
    errors?: Record<string, string[]>;
}