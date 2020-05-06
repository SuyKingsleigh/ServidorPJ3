const http = require('http');

const _host = "127.0.0.1";
const _port = 42069;
const _data = '{"date" : "21:39:45 5/4/2020", "temp" : 32, "uid" : 1}';

const getOptions = {
    host: _host,
    port: _port,
    path: '/1'
};

const postOptions = {
    host: _host,
    port: _port,
    method: "POST",
    headers: {
        'Content-Type': 'application/json',
        'Content-Length': _data.length
    }
};

var req_get = http.get(getOptions, res => {
    res.setEncoding('utf8');
    res.on('data', chunk => console.log("Chunk: ", chunk));
});
req_get.end();

var req_post = http.request(postOptions, res => {
    console.log('StatusCode: ', res.statusCode);
    res.setEncoding('utf8');
    res.on('data', chunk => console.log(chunk));
});

req_post.write(_data);
req_post.end();