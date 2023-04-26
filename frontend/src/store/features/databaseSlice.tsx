import { createSlice } from "@reduxjs/toolkit";
import axios, {AxiosResponse} from "axios";
import {databaseResetEndpoint} from "../../constants/endpoints";

const initialState = {
    success: false
};

const databaseSlice = createSlice({
    name: "database",
    initialState: initialState,
    reducers: {
        // resetDatabase(state, action) {
        //     return action.payload;
        // }
    },
});

export default databaseSlice.reducer;

/**
 * @param {(severity: string, summary: string, detail: string) => void} showToast
 */
export const resetDatabase = async () : Promise<AxiosResponse> => {
    return await axios
            .post(databaseResetEndpoint, {}, {
                headers: {
                    "Content-Type": "application/json",
                    // "Authorization": `Token ${process.env.REACT_APP_TOKEN}`
                },
                withCredentials: false,
            });
};