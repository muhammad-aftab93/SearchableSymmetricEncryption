import {useState} from 'react';
import { Menubar } from 'primereact/menubar';
import './Header.css';
import {useNavigate} from "react-router-dom";

const Header = () => {
    const [selectedItem, setSelectedItem] = useState('');
    const navigate = useNavigate();

    const items = [
        {
            label: 'Search',
            icon: 'pi pi-fw pi-search',
            command: () => onItemSelectHandler('search'),
            className: selectedItem == 'search' ? 'active' : ''
        },
        {
            label: 'Upload files',
            icon: 'pi pi-fw pi-upload',
            command: () => onItemSelectHandler('upload'),
            className: selectedItem == 'upload' ? 'active' : ''
        },
        {
            label: 'Database',
            icon: 'pi pi-fw pi-database',
            command: () => onItemSelectHandler('database'),
            className: selectedItem == 'database' ? 'active' : ''
        }
    ];

    const onItemSelectHandler = (item: string) => {
        setSelectedItem(item);
        navigate(`/${item}`)
    }

    const start = <>
        <img alt="logo" src="logo.png" height="35" className="mx-2"></img>
        <a id={"title"} href="#" onClick={() => onItemSelectHandler('')}>
            <h5 className="mt-3" style={{ display: "inline" }}>Searchable Symmetric Encryption</h5>
        </a>

    </>;

    return (
        <>
            <div className="card">
                <Menubar className="navbar" model={items} start={start} />
            </div>
        </>

    )
}

export default Header;