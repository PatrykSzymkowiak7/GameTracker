import { useState, type FormEvent } from "react";
import { type RegisterRequest } from "../types/auth";
import { register } from "../services/authService";

export interface RegisterPageProps{
    onSwitchToLogin: () => void;
}

export function RegisterPage(props: RegisterPageProps){
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    async function handleSubmit(event: FormEvent<HTMLFormElement>){
        event.preventDefault();

        setError(null);
        setSuccess(null);

        if(password !== confirmPassword){
            setError("Passwords do not match");
            return;
        }

        setIsLoading(true);

        const credentials: RegisterRequest = {
            username,
            email,
            password
        };

        try{
            await register(credentials);
            setSuccess("Registration successful! You can now log in.")
        }
        catch(error){
            if(error instanceof Error){
                setError(error.message);
            }
        }
        finally{
            setIsLoading(false);
        }
    }

    return(
        <form 
            onSubmit={handleSubmit}
            className="flex w-full max-w-sm flex-col gap-4 rounded-xl border border-slate-700 bg-slate-800 p-8">
                        
            <h1 className="text-2xl font-bold">GameTracker</h1>
            
            <input 
                required
                type="text"
                placeholder="Username"
                value={username}
                onChange={event => setUsername(event.target.value)}
                className="rounded-lg border border-slate-600 bg-slate-900 p-3 text-white"
            />
            <input
                required
                type="email"
                placeholder="Email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
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
            <input
                required
                type="password"
                placeholder="Confirm password"
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
                className="rounded-lg border border-slate-600 bg-slate-900 p-3 text-white"
           />
            <button 
                type="submit" 
                disabled={isLoading}
                className="rounded-lg bg-blue-600 p-3 font-semibold hover:bg-blue-500 disabled:opacity-50">
                {!isLoading ? "Register" : "Registering..."}
            </button>

            <div className="text-center text-sm text-slate-400">
                Do you have an account?{" "}
                <button
                    type="button"
                    disabled={isLoading}
                    className="font-semibold text-blue-400 hover:text-blue-300 hover:underline"
                    onClick={props.onSwitchToLogin}
                >
                Login
                </button>
            </div>

            {error && <p className="text-sm text-red-400">{error}</p>}
            {success && <p className="text-sm text-green-400">{success}</p>}
        </form>
    )
}