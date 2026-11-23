import { useAuth } from "../context/AuthContext";
import { LogOut } from "lucide-react";

function SideBar() {
    const { logout } = useAuth();

    return (
        <aside className="flex min-h-screen w-64 flex-col bg-slate-900 p-6 text-white">
            <h1 className="mb-10 text-2xl font-bold">
                GameTracker
            </h1>

            <nav className="flex flex-col gap-3">
                <a className="rounded-lg bg-blue-600 p-3" href="#">
                    Library
                </a>
                <a className="rounded-lg p-3 text-gray-400 hover:bg-slate-800 hover:text-white" href="#">
                    Statistics
                </a>
                <a className="rounded-lg p-3 text-gray-400 hover:bg-slate-800 hover:text-white" href="#">
                    Settings
                </a>
                <button
                    onClick={logout}
                    className="mt-auto flex w-full items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium text-red-400 transition-colors hover:bg-red-500/10 hover:text-red-300"
                >
                    <LogOut size={18} />
                    Logout
                </button>
            </nav>
        </aside>
    );
}

export default SideBar;