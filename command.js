#!/usr/bin/env node

/*
 * Module dependencies.
 */
var util = require('util');
var program = require('commander');
var child_process = require('child_process');


program
    .version('1.0.0')
    .usage('[options] [value ...]')
    .option('-c, --compile', 'compile c# file')
    .option('-r, --run', '執行 c# 程式')
    .option('-t, --test_bot', '執行 bot 測試')
    .option('-p, --pm2', '執行 pm2')
    .parse(process.argv);

// global variables ----------
// var exec_stdin = '執行命令:\n';
// var exec_stdout = '輸出:\n';
var csharp_file_name = "utilityLib";
//----------------------------


// methods ----------
var help = function(){
  var stdin = 'node '+ process.mainModule.filename + ' --help';
  child_process.exec(stdin, function(err, stdout, stderr) {
    if (err) throw err;
    console.log(stdout);
    // exec_stdin += util.format('%s\n',stdin);
    // exec_stdout += util.format('%s\n',stdout);
    // console.log(exec_stdin);
    // console.log(exec_stdout);

    process.exit();
  });
};

var compileCSharpFile = function(program){
  // console.log(program.rawArgs);
  var default_output_path = '~/mocha-test/' + csharp_file_name + ".exe";
  var output_path = program.rawArgs[3] || default_output_path;
  // var child = child_process.spawn('mcs', program.rawArgs.slice(3, program.rawArgs.length), {stdio: [0, 1, 2]});
  // var child = child_process.spawn('mcs', ["-sdk:4.5", "testCsharp.cs", "-target:library"], {stdio: [0, 1, 2]});
  var clear_old_file = child_process.spawn('rm', ["-rf", csharp_file_name +".exe"], {stdio: [0, 1, 2]});
  var child = child_process.spawn('mcs', ["-sdk:4.5", csharp_file_name+".cs"], {stdio: [0, 1, 2]});

  child.on('exit', function(){
    console.log("### compile success .\n");

    // copy 檔案到 bot 執行目錄下
    var fs = require('fs');
    fs.createReadStream(csharp_file_name+".exe").pipe(fs.createWriteStream("../mocha-test/"+csharp_file_name+".exe"));

  });
};

var runCSharpCommand = function(program){

  var spawn = require('child_process').spawn,
  mono    = spawn('mono', [csharp_file_name+".exe"].concat(program.rawArgs.slice(3, program.rawArgs.length)));

  mono.stdout.on('data', function (data) {
    console.log('stdout: ' + data);
  });
};

var runPM2 = function(program){
  console.log(program.rawArgs);
  var child = child_process.spawn('pm2', program.rawArgs.slice(3, program.rawArgs.length), {stdio: [0, 1, 2]});
  child.on('data', function(data){
    // console.log(data);
  });
};

var runBot = function(program){
  // var spawn = require('child_process').spawn,
  // bot    = spawn('node', [csharp_file_name+".exe"].concat(program.rawArgs.slice(3, program.rawArgs.length)));
  for(var i = 1; i <= 1 ; i++){
    var n = child_process.fork('../mocha-test/app.js');
    n.on('message', function(m) {
      console.log('PARENT got message:', m);
    });
    // n.send({ hello: 'world' });
  }
};



if(program.compile){
  compileCSharpFile(program);
}else if(program.run){
  runCSharpCommand(program);
}else if(program.pm2){
  runPM2(program);
}else if(program.test_bot){
  runBot(program);
}else{

  help();
	// var stdin = 'node '+ process.mainModule.filename + ' --help';
	// child_process.exec(stdin);
	// (function(){
	// 	child_process.exec(stdin, function(err, stdout, stderr) {
	// 		if (err) throw err;
	// 		exec_stdin += util.format('%s\n',stdin);
	// 		exec_stdout += util.format('%s\n',stdout);
	// 		console.log(exec_stdin);
	// 		console.log(exec_stdout);
  //
	// 		process.exit();
	// 	});
	// })
	// (stdin);

};
