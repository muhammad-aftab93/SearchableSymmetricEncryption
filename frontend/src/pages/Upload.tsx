import React, { useRef, useState} from 'react';
import { Button } from 'primereact/button';
import { ConfirmDialog} from 'primereact/confirmdialog';
import { ProgressSpinner } from "primereact/progressspinner";
import { Messages } from 'primereact/messages';
import {SseUploadFileEndpoint} from "../constants/endpoints";

const Upload = () => {
    const [loading, setLoading] = useState(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const msgs = useRef<Messages>(null);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files) {
            setSelectedFile(event.target.files[0]);
        }
    };

    const onUploadFile = () => {
        if(!selectedFile)
        {
            msgs.current?.show({
                severity: 'error', sticky: false, content: (
                    <React.Fragment>
                        <div className="ml-2">Select file.</div>
                    </React.Fragment>
                )
            });
            return;
        }

        setLoading(true);
        if (selectedFile) {
            const formData = new FormData();
            formData.append('formFile', selectedFile);
            fetch(SseUploadFileEndpoint, {
                method: 'POST',
                body: formData,
            })
            .then((response) => { return response.json(); })
            .then((result) => {
                setLoading(false);

                msgs.current?.show({
                    severity: 'success', sticky: false, content: (
                        <React.Fragment>
                            <div className="ml-2">File uploaded successfully.</div>
                        </React.Fragment>
                    )
                });

            })
            .catch((error) => {
                setLoading(false);

                msgs.current?.show({
                    severity: 'error', sticky: false, content: (
                        <React.Fragment>
                            <div className="ml-2">File upload failed.</div>
                        </React.Fragment>
                    )
                });

            });
        }
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
                    <h3 className="text-center">Upload file</h3>
                    <div className="col-3">
                        <ConfirmDialog />
                        <div className="flex flex-wrap gap-2 justify-content-center">
                            <div>
                                Select File: 
                                <input
                                    type="file"
                                    onChange={handleFileChange}
                                    accept=".txt" />
                            </div>
                            <br />
                            <div>
                                <Button
                                    onClick={onUploadFile}
                                    icon="pi pi-upload"
                                    className={"p-button-primary"}
                                    label="Upload File">
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            }

            <br />

            <Messages ref={msgs} />
        </>
    )
}

export default Upload;