import { load } from "@tauri-apps/plugin-store"
import { useCallback, useEffect, useMemo, useState } from "react"

export type StoreType = {
    value: string
}

export type FetchResult<T> = [
    data: T | null,
    error: Error | null,
    loading: boolean,
    setFetchNow: () => void
]

function useFetch<T>(endpoint: string, query?: { [key: string]: string | number }, auto: boolean = true) {
    const [url, setUrl] = useState<string>("")
    const [token, setToken] = useState<string>("")
    const triggerFetch = () => setFetchNow(true)
    const [result, setResult] = useState<FetchResult<T | null>>([null, null, true, triggerFetch])
    const [fetchNow, setFetchNow] = useState<boolean>(auto)

    useMemo(() => {
        (async () => {
            const store = await load("config.json", { autoSave: false })

            var url = await store.get<StoreType>("url")
            var token = await store.get<StoreType>("token")
            if (url && token) {
                setUrl(url.value)
                setToken(token.value)
            }
            else {
                setResult([null, {
                    name: "no_config",
                    message: "Couldn't find token or url"
                }, false, triggerFetch])
            }
        })()
    }, [])

    const fetcher = useCallback((endpoint: string, url: string, token: string, query?: { [key: string]: string | number }) => {
        var builtEndpoint = new URL(url + endpoint)
        var q = new URLSearchParams(query ? Object.entries(query).map(([key, value]) => [key, String(value)]) : []).toString()
        if (q) builtEndpoint.search = q
        fetch(builtEndpoint.toString(), {
            method: "GET",
            headers: {
                "Authorization": "Bearer " + token
            }
        })
            .then((res) => res.json())
            .then((data) => setResult([data, null, false, triggerFetch]))
            .catch((error) => setResult([null, error, false, triggerFetch]))
    }, [endpoint, query])


    useEffect(() => {
        if (url && token && fetchNow) {
            fetcher(endpoint, url, token)
        }
    }, [url, token, fetchNow])

    return [...result, setFetchNow]
}

export default useFetch