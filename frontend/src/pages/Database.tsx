import React, {useRef, useState} from 'react';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import {Button} from "primereact/button";
import {ProgressSpinner} from "primereact/progressspinner";
import {resetDatabase} from "../store/features/databaseSlice";
import { Messages } from 'primereact/messages';
import { Card } from 'primereact/card';

const Database = () => {
    const [loading, setLoading] = useState(false);
    const msgs = useRef<Messages>(null);

    const accept = () => {
        setLoading(true);
        resetDatabase()
            .then(res => {
                setLoading(false);
                if(res.data) {
                    msgs.current?.show({
                        severity: 'success', sticky: false, content: (
                            <React.Fragment>
                                <div className="ml-2">Database has been reset successfully.</div>
                            </React.Fragment>
                        )
                    });
                } else {
                    msgs.current?.show({
                        severity: 'success', sticky: false, content: (
                            <React.Fragment>
                                <div className="ml-2">Database has been reset successfully.</div>
                            </React.Fragment>
                        )
                    });
                }
            })
            .catch(err => {
                setLoading(false);
                msgs.current?.show({
                    severity: 'success', sticky: false, content: (
                        <React.Fragment>
                            <div className="ml-2">Something went wrong.</div>
                        </React.Fragment>
                    )
                });
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
            <Card style={{ backgroundColor: '#F8F9FA', height: '40rem', marginTop: '1rem' }} className="text-center">
                {loading &&
                        <div className="card flex justify-content-center">
                            <ProgressSpinner />
                        </div>
                    }

                    {!loading &&
                        <div className="row">
                            <h3 className="text-center">Database</h3>
                            <div className="col-4 mt-5"></div>
                            <div className="col-4">
                                <ConfirmDialog />
                                <div className="card flex flex-wrap gap-2 justify-content-center">
                                    <Button onClick={confirm} icon="pi pi-times" className={ "p-button-danger" } label="Reset Database"></Button>
                                </div>
                            </div>
                        </div>
                    }

                    <Messages ref={msgs} />
            </Card>
        </>
    )
}

export default Database;