import { Button } from 'primereact/button';
import { Card } from 'primereact/card';
import { InputText } from 'primereact/inputtext';
import { Messages } from 'primereact/messages';
import { ProgressSpinner } from 'primereact/progressspinner';
import { useRef, useState } from 'react';

const Search = () => {
    const [loading, setLoading] = useState(false);
    const msgs = useRef<Messages>(null);
    const [value, setValue] = useState('');

    const onSearchHandler = () => {

    }

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
            
            <div className="col-5">
                <Card title="Romeo and Juliet" className='m-5 text-center' style={{ background: '#f2f2f2' }}>
                    <img width='150px' src="/file-icon.png" alt="" />
                    <h5>Filename.txt</h5>
                    <a className='btn btn-success' href="#">Download</a>
                </Card>
            </div>
            
        </>
    );
}

export default Search;