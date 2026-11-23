import { type AuthContextType } from "../types/auth";
import { createContext, useContext, useState, type ReactNode } from "react";

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [token, setToken] = useState<string | null>(() => {
        const storedToken = localStorage.getItem("accessToken");
        const expiresAt = localStorage.getItem("tokenExpiresAtUtc");

        if(!storedToken || !expiresAt){
            return null;
        }
        
        const expirationTime = new Date(expiresAt).getTime();

        if(!Number.isFinite(expirationTime) || expirationTime <= Date.now()){
            localStorage.removeItem("accessToken");
            localStorage.removeItem("tokenExpiresAtUtc");
            return null;
        }

        return storedToken;
    });

    function logout(){
        localStorage.removeItem("accessToken");
        localStorage.removeItem("tokenExpiresAtUtc");
        setToken(null);
    }

    return (
        <AuthContext.Provider value={{ token, setToken, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth(){
    const context = useContext(AuthContext);

    if(!context){
        throw new Error("use Auth must be used within AuthProvider");
    }

    return context;
}