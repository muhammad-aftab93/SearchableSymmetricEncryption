import { Card } from 'primereact/card';

const Home = () => {
    return <>
        <div className="row" style={{ marginTop: '1rem' }}>
            <div className="col-12">

                <Card style={{ backgroundColor: '#F8F9FA', height: '30rem' }} title="Searchable Symmetric Encryption">
                    <p className="m-0">
                        Searchable symmetric encryption (SSE) is a cryptographic technique that enables searching over encrypted data while preserving the privacy and security of the data. In SSE, data is encrypted before being outsourced to a server or a cloud, and the encrypted data can be searched for specific keywords or patterns without decrypting the data.
                        <br /><br />
                        The basic idea behind SSE is to split the data into a set of searchable tokens, which are encrypted and then stored on a server. When a user wants to search for a specific keyword, they encrypt the keyword using the same encryption scheme as used for the tokens, and send the encrypted keyword to the server. The server can then search for the encrypted keyword among the encrypted tokens, and return the matching encrypted tokens to the user. The user can then decrypt the returned encrypted tokens to obtain the corresponding plaintext data.
                        <br /><br />
                        SSE techniques can be categorized into two main types: single-key SSE and multi-key SSE. In single-key SSE, a single key is used to encrypt and search the data. In multi-key SSE, multiple keys are used to encrypt different parts of the data, which can be searched separately.
                        <br /><br />
                        SSE has a wide range of applications, including in cloud computing, database systems, and mobile devices. It is particularly useful in scenarios where sensitive data needs to be stored and searched, but the data owner wants to maintain the privacy and security of the data.
                    </p>
                </Card>
            </div>
        </div>
    </>
}

export default Home;