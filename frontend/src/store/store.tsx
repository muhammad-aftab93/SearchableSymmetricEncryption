import { configureStore } from "@reduxjs/toolkit";
import databaseSlice from '../store/features/databaseSlice'


const store = configureStore({
    reducer: {
        database: databaseSlice,
    },
});

export default store;
