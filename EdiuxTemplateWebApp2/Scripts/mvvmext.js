/// <reference path="jquery-3.1.1.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-1.11.1.min.js" />
/// <reference path="jquery-migrate-1.2.1.min.js" />
/// <reference path="jquery-ui.min.js" />
/// <reference path="mvvm.js" />

//DataModel
var mvvmDataModel = function (bindingname,jqItem) {
    this.__value = jqItem.val();
    this.__datasource = [];
    this.__jqObject = jqItem;
    this.__bindingName = bindingname;
    $(this.__jqObject).change(function () {   });
}

mvvmDataModel.prototype = {
    get Value: function () { return this.__value;},
    set Value: function (value) {
        this.__value = value;
        this.PropertyChanged();
    },
    get DataSource: function () { return this.__datasource; },
    set DataSource: function (value) { this.__datasource = value; this.DataBinding();},
    get DOM: function () { return this.__jqObject; },
    get bindingName: function () { return this.__bindingName;}
};

mvvmDataModel.prototype.PropertyChanged = function () {
    console.log('PropertyChanged:' + this.__bindingName);
    var coVal = this.__value;
    if ($.isArray(this.__jqObject)) {
        $(this.__jqObject).each(function (i, item) {
            //判斷是否為CheckBox
            if ($(item).is('input:checkbox')) {
                item.checked = coVal;   //將checkbox設定是否選
            }
            //判斷是否為下拉式選單
            if ($(item).is('select')) {
                $(item).children("option:selected").each(function (i, v) { $(v).removeProp('selected'); });
                $(item).children("option").each(function (i, v) {
                    if ($(v).val() == coVal) {
                        $(v).prop('selected', true);
                    }
                });
            }
        });
    } else {
        if ($(this.__jqObject).is('input:checkbox')) {
            this.__jqObject.checked = this.__value;
        }
        if ($(this.__jqObject).is('select')) {
            
        }
    }
}

//將資料來源內容注入對應的控制項
mvvmDataModel.prototype.DataBinding = function () {

};

//重新封裝物件mvvm
var mvvm2 = function () { };

mvvm2.prototype = mvvm;
mvvm2.prototype.captalize = function () {
    return this.replace(/(^|\s)([a-z])/g, function (m, p1, p2) { return p1 + p2.toUpperCase(); });
};
mvvm2.prototype.CriadorDeGetAndSetter = function (obj) {

    if (typeof (CriadorDeGetAndSetter) !== 'undefined') {
        if ($.isFunction(CriadorDeGetAndSetter)) {
            CriadorDeGetAndSetter(obj);
        }
    }

    var captalize = this.captalize;

    for (var prop in obj) {
        if ($.isFunction(obj[prop]))
            continue;
        (function (privatename, proto) {
            var propname =  captalize.call(privatename.replace("_", ""));
            var newinstance = new mvvmDataModel(propname, $('[data-bind=' + propname + ']'));
            newinstance.Value = obj[privatename]; 
            obj[propname] = newinstance;
            //Object.defineProperty(proto, propname, {
            //    get: function () {
            //        return obj[privatename].Value;
            //    },
            //    set: function (value) {
            //        obj[privatename].Value = value;
            //        obj["PropertyChanged"](propname);
            //    }
            //});
        })(prop, obj);
    }
    obj["PropertyChanged"] = function (name) {
        obj[name].PropertyChanged();
    };
}

mvvm2.prototype.observe = function (obj) {
    this.CriadorDeGetAndSetter(obj);
    //找出所有具有 data-bind 屬性的元素
    $('[data-bind]').each(function (index, item) {
        var bindingName = $(item).attr('data-bind');
        var eventName = $(item).attr('data-event');
        var autochange = $(item).attr('data-change');
        var ajaxSuccess_Fn = $(item).attr('data-ajaxsuccess');
        var ajaxFail_Fn = $(item).attr('data-ajaxerror');
        var tagName = $(item).prop("tagName");

    });
};



(function (mvvm) {
    mvvm.CriadorDeGetAndSetter=function(obj){},
    mvvm.observe = function (obj) {
        //先執行原本版本的綁定作業
        CriadorDeGetAndSetter(obj);
       
        //找出所有具有 data-bind 屬性的元素
        $('[data-bind]').each(function (index, item) {
            //取得data-bind屬性的設定值
            var dataBindField = $(item).attr("data-bind");

            //如果data-bind沒有設定名稱，先檢查id
            if (dataBindField == '') {
                dataBindField = $(item).attr("id");
            }

            //如果data-bind沒有設定名稱，先檢查name
            if (dataBindField == '') {
                dataBindField = $(item).attr("name");
            }

            //如果data-bind沒有設定名稱，先檢查tagName
            if (dataBindField == '') {
                dataBindField = $(item).attr("tagName") + index;
            }

            obj[dataBindField] = null;

            $(item).change(function () {
                //檢查是否為checkbox
                if ($(this).is("input:checkbox")) {
                    obj[dataBindField] = this.checked;
                }
                else {
                    if ($(this).is("select")) {
                        obj[dataBindField] = $(this).children("option:selected").prop('selected');
                    } else {
                        obj[dataBindField] = $(this).val();
                    }
                }
                var changefn = $(this).attr('data-change');
                if (typeof (changefn) !== 'undefined') {
                    changefn = eval(changefn);
                    changefn(this, obj);
                }
            });

            obj["PropertyChanged"] = function (nome) {

                if ($('[data-bind=' + nome + ']').is(":checkbox")) {
                    $('[data-bind=' + nome + ']').each(function (index,
 item) {
                        $(item).checked = obj[nome];
                    });
                }
                else {
                    if ($('[data-bind=' + nome + ']').is('input:radio')) {
                        if (obj[nome] != null && obj[nome] != '') {
                            $('[data-bind=' + nome + ']').filter('[value='
 + obj[nome] + ']').prop('checked', true);
                        }
                    }
                    else {
                        if ($('[data-bind=' + nome + ']').is("select")) {
                            $('[data-bind=' + nome + ']').empty();
                            for (var name in obj[nome]) {
                                $('[data-bind=' + nome + ']').append
 ('<option value=\'' + obj[nome][name] + '\'>' + name + '</option>');
                            }
                        } else {
                            $('[data-bind=' + nome + ']').val(obj[nome]);
                        }
                    }

                }
            };
        });
        //當點擊回應的處理
        $('[data-ajax]').each(function (index, item) {
            $(item).click(function () {
                var url = location.pathname + '/' + $(item).attr
 ("data-ajax");
                var callbackfn = $(item).attr("data-ajaxsuccess");
                var failcallbackfn = $(item).attr("data-ajaxerror");

                var paramObj = obj;

                if ($(item).attr("data-scope")) {
                    //指定有限範圍的參數傳遞
                    var scopename = $(item).prop('data-scope');
                    $('[data-scope=' + scopename + ']').each(function (index, item) {

                    });
                }

                if ($(this).attr('data-ajax')) {
                    //當點擊時套用AJAX回呼
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: JSON.stringify(obj),
                        success: function (data) {
                            var result = {};

                            if (data.d) {
                                result = JSON.parse(data.d);
                            }
                            else {
                                result = JSON.parse(data);
                            }

                            $('[data-bind]').each(function (index, item) {
                                var dataBindField = $(item).attr
 ("data-bind");

                                if (result.HasError) {
                                    return;
                                }

                                var _callbackvalue = result.data
 [dataBindField];

                                if (_callbackvalue != null || typeof
 (_callbackvalue) !== 'undefined') {
                                    obj[dataBindField] = _callbackvalue;
                                    obj["PropertyChanged"](dataBindField);
                                }

                            });

                            if (result.HasError) {
                                if (typeof (failcallbackfn) !==
 'undefined') {
                                    failcallbackfn = eval(failcallbackfn);

                                    if ($.isFunction(failcallbackfn)) {
                                        failcallbackfn(result);
                                    }
                                    else {
                                        alert(result.ErrorMessage);
                                    }
                                }

                                return;
                            }

                            callbackfn = eval(callbackfn);

                            if ($.isFunction(callbackfn)) {
                                callbackfn(result);
                            }
                        },
                        error: function (data) {

                            if (data.responseText) {
                                data = JSON.parse(data.responseText);
                            }
                            else {
                                if (data.d) {
                                    data = JSON.parse(data.d);
                                } else {
                                    data = JSON.parse(data);
                                }
                            }

                            failcallbackfn = eval(failcallbackfn);
                            if ($.isFunction(failcallbackfn)) {
                                failcallbackfn(data);
                            } else {
                                if (typeof (data.Message) !== 'undefined') {
                                    alert(data.Message);
                                }
                                else {
                                    alert(data);
                                }
                            }
                        },
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json'
                    });
                }
            });
        });
        //掃描按鈕
        $(":button").each(function (index, item) {
            if ($(item).attr('data-event')) {
                $(item).click(function () {
                    var eventElementId = $(item).attr('data-event');
                    if (typeof (eventElementId) !== 'undefined') {
                        var funcallBack = eval(eventElementId);
                        if ($.isFunction(funcallBack)) {
                            funcallBack(this, obj);
                        }
                    }
                });

            }
        });
        //掃描超連結(特定行為綁定)
        $('a').each(function (index, item) {
            if ($(item).attr('data-ajax')) {
                $(item).removeAttr('href');
            }
            if ($(item).attr('data-event')) {
                $(item).removeAttr('href');
                $(item).click(function () {
                    var eventElementId = $(this).attr('data-event');
                    if (typeof (eventElementId) !== 'undefined') {
                        var funcallBack = eval(eventElementId);
                        if (typeof (funcallBack) !== 'undefined') {
                            if ($.isFunction(funcallBack)) {
                                funcallBack(this, obj);
                            }
                        }
                    }

                });

            }

        });
        //取得表單[form]的元素使用AJAX資料來源綁定
        $('form').each(function (index, item) {
            var callbackfn = $(item).attr("data-ajaxsuccess");
            var failcallbackfn = $(item).attr("data-ajaxerror");

            if (typeof (obj.forms) === 'undefined') {
                obj["forms"] = {};
            }

            var _form = {};

            if ($(this).prop('id')) {
                var _newformname = $(this).prop('id');
                obj.forms[_newformname] = {};
            }

            if ($(item).attr('data-source')) {
                //取得要取得資料的網頁方法
                var datasource = window.location.pathname + '/' + $
 (item).attr('data-source');

                $.ajax({
                    type: 'POST',
                    url: datasource,
                    data: JSON.stringify(obj),
                    success: function (data) {
                        var result = {};

                        if (data.d) {
                            result = JSON.parse(data.d);
                        }
                        else {
                            result = JSON.parse(data);
                        }

                        $('[data-bind]').each(function (indexb, itemb) {
                            if ($(itemb).attr("data-bind")) {

                                var dataBindField = $(itemb).attr
 ("data-bind");

                                var _callbackvalue = result.data
 [dataBindField];

                                //  obj.forms[_newformname][databind] =
                                _callbackvalue;
                                obj[dataBindField] = _callbackvalue;
                                obj["PropertyChanged"](dataBindField);
                            }

                        });

                        callbackfn = eval(callbackfn);

                        if ($.isFunction(callbackfn)) {
                            callbackfn(result);
                        }
                    },
                    error: function (data) {
                        if (data.responseText) {
                            data = JSON.parse(data.responseText);
                        }
                        else {
                            if (data.d) {
                                data = JSON.parse(data.d);
                            } else {
                                data = JSON.parse(data);
                            }
                        }

                        failcallbackfn = eval(failcallbackfn);
                        if ($.isFunction(failcallbackfn)) {
                            failcallbackfn(data);
                        } else {
                            if (typeof (data.Message) !== 'undefined') {
                                alert(data.Message);
                            }
                            else {
                                alert(data);
                            }
                        }
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json'
                });
            }
        });
    }
})(mvvm);

(function ($) {
    //static extend
    $.extend({
        mvvm: function () {
            mvvm.observe(App);
        },
        raiseChangeEvent: function (bindingid, value) {
            if (typeof (value) !== 'undefined') {
                App[bindingid] = value;
            }
            App.PropertyChanged(bindingid);
        },
        ajaxPost: function (method, data, success, fail) {
            $.ajax({
                type: 'POST',
                url: location.pathname + '/' + method,
                data: JSON.stringify(data),
                success: function (serverdata) {
                    var result = JSON.parse(serverdata);
                    success = eval(success);
                    if ($.isFunction(success)) {
                        success(result);
                    }
                },
                error: function (data) {
                    if (data.responseText) {
                        data = JSON.parse(data.responseText);
                    }
                    else {
                        if (data.d) {
                            data = JSON.parse(data.d);
                        } else {
                            data = JSON.parse(data);
                        }
                    }

                    fail = eval(fail);
                    if ($.isFunction(fail)) {
                        fail(data);
                    } else {
                        if (typeof (data.Message) !== 'undefined') {
                            alert(data.Message);
                        }
                        else {
                            alert(data);
                        }
                    }
                },
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            });
        }
    });
})(jQuery);