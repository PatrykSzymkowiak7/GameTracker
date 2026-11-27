import { login } from "../services/authService";
import { useState } from "react";
import { useAuth } from "../context/AuthContext";

interface LoginPageProps{
    onSwitchToRegister: () => void;
}

function LoginPage(props: LoginPageProps) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const { setToken } = useAuth();

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        try {
            setIsLoading(true);
            setError(null);

            const response = await login({ username, password });
            localStorage.setItem("accessToken", response.accessToken);
            localStorage.setItem("tokenExpiresAtUtc", response.expiresAtUtc)
            setToken(response.accessToken);
        }
        catch (error) {
            if (error instanceof Error) {
                setError(error.message);
            } else {
                setError("Login failed");
            }
        }
        finally {
            setIsLoading(false);
        }
    }

    return (
        <form
            onSubmit={handleSubmit}
            className="flex w-full max-w-sm flex-col gap-4 rounded-xl border border-slate-700 bg-slate-800 p-8"
        >
            <h1 className="text-2xl font-bold">GameTracker</h1>

            <input
                required
                type="text"
                placeholder="Username"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                className="rounded-lg border border-slate-600 bg-slate-900 p-3 text-white"
            />

            <input
                required
                type="password"
                placeholder="Password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                className="rounded-lg border border-slate-600 bg-slate-900 p-3 text-white"
            />

            <button
                type="submit"
                disabled={isLoading}
                className="rounded-lg bg-blue-600 p-3 font-semibold hover:bg-blue-500 disabled:opacity-50"
            >
                {isLoading ? "Logging in..." : "Login"}
            </button>

            <div className="text-center text-sm text-slate-400">
                Don't have an account?{" "}
                <button
                    type="button"
                    disabled={isLoading}
                    className="font-semibold text-blue-400 hover:text-blue-300 hover:underline"
                    onClick={props.onSwitchToRegister}
                >
                Register
                </button>
            </div>

            {error && <p className="text-red-500">{error}</p>}
        </form>
    );
}

export default LoginPage;
