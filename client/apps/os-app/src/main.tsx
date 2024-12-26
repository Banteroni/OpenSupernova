import React from "react";
import ReactDOM from "react-dom/client";
import App from "./app";
import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import "./index.css";
import Login from "./views/login";
import Home from "./views/home";
import { Provider } from "react-redux";
import store from "./store";

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to="/app" />} />
          <Route path="/login" element={<Login />} />
          <Route path="/app" element={<App />} >
            <Route index element={<Home />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </Provider>
  </React.StrictMode>,
);
