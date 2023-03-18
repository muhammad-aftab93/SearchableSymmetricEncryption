import React, {useRef, useState} from 'react';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import {Button} from "primereact/button";
import {ProgressSpinner} from "primereact/progressspinner";
import {resetDatabase} from "../store/features/databaseSlice";

const Database = () => {
    const [loading, setLoading] = useState(false);

    const accept = () => {
        setLoading(true);
        resetDatabase()
            .then(res => {
                setLoading(false);
                if(res.data) {
                    alert('Database has been reset successfully.')
                } else {
                    alert('Database has been reset successfully.')
                }
            })
            .catch(err => {
                setLoading(false);
                alert('Something went wrong.');
            });
    }

    const reject = () => {
        // Do nothing for now.
        //toast.current.show({ severity: 'warn', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
    }

    const confirm = () => {
        confirmDialog({
            message: 'Do you want to reset database?',
            header: 'Reset Confirmation',
            icon: 'pi pi-info-circle',
            acceptClassName: 'p-button-danger',
            accept,
            reject
        });
    };

    return (
        <>
            {loading &&
                <div className="card flex justify-content-center">
                    <ProgressSpinner />
                </div>
            }

            {!loading &&
                <div className="row" style={{ marginTop: '1rem' }}>
                    <div className="col-3">
                        <ConfirmDialog />
                        <div className="card flex flex-wrap gap-2 justify-content-center">
                            <Button onClick={confirm} icon="pi pi-times" className={ "p-button-danger" } label="Reset Database"></Button>
                        </div>
                    </div>
                </div>
            }
        </>
    )
}

export default Database;