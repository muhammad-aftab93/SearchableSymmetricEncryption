import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import './App.css';
import Layout from "./layout/Layout";
import Search from "./pages/Search";
import Home from "./pages/Home";
import NotFound from "./pages/NotFound";
import Upload from "./pages/Upload";
import Database from "./pages/Database";

function App() {
  return (
    <Router>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/" element={<Home />} />
        </Route>
        <Route element={<Layout />}>
          <Route path="/home" element={<App />} />
        </Route>
        <Route element={<Layout />}>
          <Route path="/search" element={<Search />} />
        </Route>
        <Route element={<Layout />}>
          <Route path="/upload" element={<Upload />} />
        </Route>
        <Route element={<Layout />}>
          <Route path="/database" element={<Database />} />
        </Route>
        <Route path="/*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;
