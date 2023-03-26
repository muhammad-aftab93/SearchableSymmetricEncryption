import { Button } from 'primereact/button';
import { Card } from 'primereact/card';
import { InputText } from 'primereact/inputtext';
import { Messages } from 'primereact/messages';
import { ProgressSpinner } from 'primereact/progressspinner';
import React from 'react';
import { useRef, useState } from 'react';
import { SseDownloadFileEndpoint, SseSearchFileEndpoint } from '../constants/endpoints';
import fileDownload from 'js-file-download';

const Search = () => {
    const [loading, setLoading] = useState(false);
    const msgs = useRef<Messages>(null);
    const [value, setValue] = useState('');
    const [response, setResponse] = useState({
        message: '',
        success: false,
        returnedObject: {
            fileContent: null,
            fileName: null,
            url: ''
        }
    });

    const onSearchHandler = () => {
        setLoading(true);
        const formData = new FormData();
        formData.append('fileName', value);
        fetch(SseSearchFileEndpoint, {
            method: 'POST',
            body: formData,
        })
        .then((response) => { return response.json(); })
        .then((result) => {
            setLoading(false);
            setResponse(result);

            if(!result.success) {
                msgs.current?.show({
                    severity: 'error', sticky: false, content: (
                        <React.Fragment>
                            <div className="ml-2">{result.message}</div>
                        </React.Fragment>
                    )
                });
            }
        })
        .catch((error) => {
            setLoading(false);

            msgs.current?.show({
                severity: 'error', sticky: false, content: (
                    <React.Fragment>
                        <div className="ml-2">{error.message}</div>
                    </React.Fragment>
                )
            });

        });
    }

    const onDownloadFile = (filename: string | null) => {
        if (!filename) return;

        setLoading(true);

        fetch(SseDownloadFileEndpoint + '/' + filename)
        .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
            return response.blob();
        })
        .then(blob => {
            setLoading(false);

            fileDownload(blob, filename)

            msgs.current?.show({
                severity: 'success', sticky: false, content: (
                    <React.Fragment>
                        <div className="ml-2">File downloaded successfully. {filename}</div>
                    </React.Fragment>
                )
            });

        })
        .catch(error => {
            setLoading(false);

            msgs.current?.show({
                severity: 'error', sticky: false, content: (
                    <React.Fragment>
                        <div className="ml-2">{error.message}</div>
                    </React.Fragment>
                )
            });

        });

    }

    const getTitle = (filename: any) => filename.substring(0, filename.lastIndexOf('.'));

    return (
        <>
            {loading &&
                <div className="card flex justify-content-center">
                    <ProgressSpinner />
                </div>
            }

            {!loading &&
                <div className="row" style={{ marginTop: '1rem' }}>
                    <div className="flex justify-content-center">
                        <h3 className="text-center">Search file</h3>
                        
                        Search by filename:&nbsp;

                        <InputText value={value} onChange={(e) => setValue(e.target.value)} />

                        &nbsp;
                        <Button
                            type="button"
                            onClick={onSearchHandler}
                            label="Search"
                            className="mt-2"
                            />

                    </div>
                </div>
            }
            
            {response.success &&
                <div className="col-5">
                    <Card
                        title={getTitle(response.returnedObject.fileName)}
                        className='m-5 text-center'
                        style={{ background: '#f2f2f2' }}
                        >
                        <img width='150px' src="/file-icon.png" alt="" />
                        <h5>{response.returnedObject.fileName}</h5>

                        <Button
                            type="button"
                            onClick={() => onDownloadFile(response.returnedObject.fileName)}
                            label="Download"
                            className="mt-2"
                            />

                    </Card>
                </div>
            }
            
            <Messages ref={msgs} />
        </>
    );
}

export default Search;