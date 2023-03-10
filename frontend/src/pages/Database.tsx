import React, { useRef } from 'react';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Toast } from 'primereact/toast';
import {Button} from "primereact/button";

const Database = () => {
    const toast = useRef<any>(null);

    const accept = () => {
        toast.current.show({ severity: 'info', summary: 'Confirmed', detail: 'You have accepted', life: 3000 });
    }

    const reject = () => {
        toast.current.show({ severity: 'warn', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
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
            <div className="row" style={{ marginTop: '1rem' }}>
                <div className="col-3">
                    <Toast ref={toast} />
                    <ConfirmDialog />
                    <div className="card flex flex-wrap gap-2 justify-content-center">
                        <Button onClick={confirm} icon="pi pi-times" className={ "p-button-danger" } label="Reset Database"></Button>
                    </div>
                </div>
            </div>
        </>
    )
}

export default Database;