import { useEffect } from "react";
import useFetch from "../../hooks/useFetch";
import HomeComponent from "./HomeComponent";
import { useNavigate } from "react-router";

export default function Home() {
    const navigate = useNavigate()
    const [data, error, loading] = useFetch<Albums>("/api/albums")

    useEffect(() => {
        if (error) {
            navigate("/login")
        }
    }, [error])
    return <HomeComponent />
}