/// <reference path="jquery-3.1.1.js" />
/// <reference path="jquery.validate.js" />

function CriadorDeGetAndSetter(obj) {
    var captalize = function () { return this.replace(/(^|\s)([a-z])/g, function (m, p1, p2) { return p1 + p2.toUpperCase(); }); };

    for (var prop in obj) {
        if ($.isFunction(obj[prop]))
            continue;
        (function (privatename, proto) {
            var propname = captalize.call(privatename.replace("_", ""));
            Object.defineProperty(proto, propname, {
                get: function () {
                    return obj[privatename];
                },
                set: function (value) {
                    obj[privatename] = value;
                    obj["PropertyChanged"](propname);
                }
            });
        })(prop, obj);
    }
    obj["PropertyChanged"] = function (name) {
        alert(name);
    };
}

var mvvm = {
    observe: function (obj) {
        CriadorDeGetAndSetter(pessoa);

        $('[data-bind]').each(function (index, item) {
            var dataBindField = $(item).attr("data-bind");
            $(item).change(function () {
                var functionsToBeCalled = [];
                for (var v in obj) {
                    if (v.indexOf("__") !== -1 && $.isFunction(obj[v]))
                        functionsToBeCalled.push(v);
                }
                obj[dataBindField] = $(this).val();
                for (var i = 0; i < functionsToBeCalled.lenght; i++) {
                    obj[functionsToBeCalled[v]].call(obj);
                }
            });
            obj["PropertyChanged"] = function (nome) {
                $('[data-bind=' + nome + ']').val(obj[nome]);
            };
        });
    }
};