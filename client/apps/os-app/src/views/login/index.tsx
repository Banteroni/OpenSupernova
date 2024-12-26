import { useNavigate } from "react-router";
import LoginComponent from "./LoginComponent";
import { load } from "@tauri-apps/plugin-store";


export default function Login() {

    let navigate = useNavigate();
    async function handleSubmit(values: { [key: string]: string }) {
        const store = load("config.json", { autoSave: false })
        var response = await fetch(`${values.serverUrl}/api/users/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: values.username,
                password: values.password
            })
        }
        )
        if (response.status === 200) {
            var data = await response.json()
            if (data.token) {
                (await store).set('token', { value: data.token });
                (await store).set('url', { value: values.serverUrl });
                navigate("/app")
            }
        } else {
            alert("Invalid credentials")
        }
    }

    function handleValidate(values: { [key: string]: string }): { [key: string]: string } {
        var errors: { [key: string]: string } = {};
        if (!values.username) {
            errors.username = "Username is required"
        }
        if (!values.password) {
            errors.password = "Password is required"
        }
        if (!values.serverUrl) {
            errors.serverUrl = "Server URL is required"
        }
        if (!values.serverUrl.includes("http://") && !values.serverUrl.includes("https://")) {
            errors.serverUrl = "Declare the protocol"
        }
        return errors
    }
    return <LoginComponent validateLogin={handleValidate} submitLogin={handleSubmit} />
}