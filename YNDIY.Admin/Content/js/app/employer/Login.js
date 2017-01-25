require.config({
    baseUrl: "/Content/js",
    paths: {
        "jquery": "jquery-1.7.2",
        'domReady': 'domReady'
    }
});
require(['jquery', 'domReady'], function ($) {
    //显示验证码
    getCaptchaHtml();
    //加载cookie账号密码
    initCookie()
    //载入图标gif 关闭
    function _hide_loading() {
        $("._loading").hide();
    }
    //载入图标gif 打开
    function _show_loading() {
        $("._loading").show();
    }
    $('form[name=_login]').on('focus', '.pwd input', function () {
        $(this).prop('type', 'password');
    });
    $('form[name=_login]').on('focus', '._error', function () {
        $(this).val('').removeClass('_error');
    });
    $('form[name=_login]').on('focus', 'input', function () {
        $('._error').empty();
    });
    $('form[name=_login]').on('click', 'input[type="checkbox"]', function () {
        var rem = $("input[type='checkbox']:checked").val();
        if (rem != 1) {
            cleanCookie('user_info');
        }
    });

    //获取验证码信息
    function getCaptchaHtml() {
        _show_loading();
        $.ajax({
            type: "get",
            url: "/Login/GetCode",
            dataType: "html",
            success: function (response) {
                _hide_loading();
                $("._code_pic").html(response);
            },
            erro: function (e) {
                _hide_loading();
                alert("网络故障");
            }
        });
    }
    //验证账号
    function Account() {
        var account = $("input[name='account']").val();
        if (/^.{4,16}$/.test(account)) {
            return true;
        } else {
            $("input[name='account']").val("请输入用户帐号,长度不小于4个字符").addClass('_error');
            return false;
        }
    }
    //验证密码
    function Password() {
        var password = $("input[name='password']").val();
        if (/^[\s\S]{5,16}$/.test(password)) {
            return true;
        } else {
            $("input[name='password']").prop('type', 'text').val("请输入密码,长度5-20个字符").addClass('_error');
            return false;
        }
    }
    //验证验证码
    function ValidCode() {
        var validCode = $("input[name='_mvcCaptchaText']").val();
        if (/^[a-zA-Z0-9]{4}$/.test(validCode)) {
            return true;
        } else {
            $("input[name='_mvcCaptchaText']").val("请输入验证码,长度为4个字符").addClass('_error');
            return false;
        }
    }
    //初始cookie密码
    function initCookie() {
        var str = getCookie('user_info');
        if (str !== null) {
            str = str.split('^');
            $("input[name='account']").val(str[0]);
            $("input[name='password']").val(str[1]);
            $("input[type='checkbox']").attr('checked', 'checked');
        }
    }
    //获取cookie密码
    function getCookie(c_name)      //根据分隔符每个变量的值
    {
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=")
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                c_end = document.cookie.lastIndexOf("^", c_start);
                if (c_end == -1)
                    c_end = document.cookie.length;
                return unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return null;
    }
    //设置cookie密码
    function setCookie(c_name, acount, pwd, expiredays)        //设置cookie
    {
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + expiredays);
        document.cookie = c_name + "=" + escape(acount) + "^" + escape(pwd) + ((expiredays == null) ? "" : "^;expires=" + exdate.toGMTString());
    }
    //检查cookie密码
    function checkCookie()      //检测cookie是否存在，如果存在则直接读取，否则创建新的cookie
    {
            setCookie('user_info', $("input[name='account']").val(), $("input[name='password']").val(), 365);
    }
    //清除cookie密码
    function cleanCookie(c_name) {     //使cookie过期
        document.cookie = c_name + "=" + ";expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
    //登录
    function login() {
        if (Account() && Password() && ValidCode()) {
            _show_loading();
            $.ajax({
                type: "post",
                url: "/login/WebLogin",
                data: $("form[name='_login']").serialize(),
                dataType: "json",
                success: function (result) {
                    _hide_loading();
                    if (result.code === 0) {
                        //失败
                        getCaptchaHtml();
                        $("._message").text(result.message).addClass('_error');
                    } else {
                        var rem = $("input[type='checkbox']:checked").val();
                        if (rem == 1) {
                            checkCookie();
                        } else {
                            cleanCookie('user_info');
                        }
                        window.location.href = result.redirectUrl;
                    }
                },
                error: function (e) {
                    _hide_loading();
                    getCaptchaHtml();
                    $("._message").text("系统异常,请重新登录").addClass('_error');
                }
            });
        }
    }
    //登录按钮 事件
    $("._login_btn").on("click", function () {
        login();
    });
    //验证码输入框回车事件
    $(".vali input[name='_mvcCaptchaText']").on("keyup", function (e) {
        if (e.keyCode == 13) {
            login();
        }
    });
});