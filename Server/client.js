const http = require('http');

const _host = "127.0.0.1";
const _port = 42069;
// const _data = '{"4/2020", "temp" : 32, "uid" : 1}'; // com erro
const _data = '{"date" : "21:39:45 18/8/2020", "temp" : 32, "umidade": 66, "uid" : 1}';
var _uid = 1; 

const getOptions = {
    host: _host,
    port: _port,
    path: '/' + _uid
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

const _put = '{"id" : 3, "nome" : "Joe Rogan"}';

const putOptions = { 
    host : _host, 
    port : _port, 
    method: "PUT", 
    headers: {
        'Content-Type' : 'application/json', 
        'Content-Length' : _put.length
    }
};

// var req_put  = http.request(putOptions, res => {
//     console.log('StatusCode :', res.statusCode);
//     res.setEncoding('utf8');
//     res.on('data', d => console.log(d));
// });
//
// req_put.write(_put);
// req_put.end();

// for (let i = 0; i < 500; i++) {
//     _uid = (i%2)+1;
//     var req_get = http.get(getOptions, res => {
//         res.setEncoding('utf8');
//         // res.on('data', chunk => console.log("Chunk: ", chunk));
//         res.on('data', chunk => console.log("i: " + i));
//     });
//     req_get.end();
// }

var req_post = http.request(postOptions, res => {
    console.log('StatusCode: ', res.statusCode);
    res.setEncoding('utf8');
    res.on('data', chunk => console.log(chunk));
});

req_post.write(_data);
req_post.end();