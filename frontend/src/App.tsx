import GamesPage from "./pages/GamesPage";
import Sidebar from "./components/SideBar";
import LoginPage from "./pages/LoginPage";
import { useAuth } from "./context/AuthContext";
import { useState } from "react";
import { RegisterPage } from "./pages/RegisterPage";

function App() {
    const { token } = useAuth();
    const [isRegistering, setIsRegistering] = useState(false);

    if (!token) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-950 text-white">
                {isRegistering ? 
                    <RegisterPage onSwitchToLogin={() => setIsRegistering(false)}/> : 
                    <LoginPage onSwitchToRegister={() => setIsRegistering(true)}/>}
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-950 text-white">
            <div className="flex">
                <Sidebar />
                <main className="flex-1 p-8">
                    <GamesPage />
                </main>
            </div>
        </div>
    );
}

export default App;