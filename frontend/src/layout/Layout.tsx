import { Outlet} from "react-router-dom";
import React, {useEffect, useRef, useState} from "react";
import Header from "../components/Header";
import { useDispatch, useSelector } from "react-redux";
import {ProgressSpinner} from "primereact/progressspinner";

const Layout = () => {

return (
    <>
        <div className="container">
            <Header />
            <Outlet />
        </div>
    </>
  );
};
export default Layout;
