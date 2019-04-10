var pug = require('pug');

module.exports = function (callback, template, model) {
	var pugCompiledFunction = pug.compile(template);
	callback(null, pugCompiledFunction(model));	
};