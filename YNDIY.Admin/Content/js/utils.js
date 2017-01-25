define([],function () {
    return {
        json2str: function (o) {
            var arr = [], _me = this;
            var fn = function (e) {
                if (typeof e == "undefined") {
                    return '""';
                } else if (typeof e == 'object' && e != null) {
                    return _me.json2str(e);
                } else {
                    return /^(string|number)$/.test(typeof e) ? '"' + e + '"' : e;
                }
            }
            for (var i in o) {
                if (typeof o[i] != "function" && !(o[i] instanceof jQuery)) {
                    if (o.push) {
                        arr.push(fn(o[i]));
                    } else {
                        arr.push('"' + i + '":' + fn(o[i]));
                    }
                }
            }
            if (o.push) {
                return '[' + arr.join(',') + ']';
            }
            return '{' + arr.join(',') + '}';
        }
    }
});
