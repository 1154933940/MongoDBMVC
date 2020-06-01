/**

**/
$(function () {
    /***操作结果用　弹出层提示　　开始***************************************/
    if ($("#ResultMsgDiv").length == 0) {
        var thisDiv = document.createElement("div")
        thisDiv.id = "ResultMsgDiv";
        document.body.appendChild(thisDiv);
    }
    $("#ResultMsgDiv").hide();

    window.u_alert = function (msg) {
        showResultMsg({ Result: 0, Msg: msg });
    }
    window.showResultMsg = function (data) {
        if (typeof (data) == "string") {
            eval("data =" + data);
        }
        var top = 0;
        var left = ($(window).width() - $("#ResultMsgDiv").width()) / 2;
        var scrollTop = $(document).scrollTop() + 10;
        var scrollLeft = $(document).scrollLeft();
        $("#ResultMsgDiv").css({ position: 'absolute', top: top + scrollTop, left: left + scrollLeft });

        //顯示提示的總時間
        var totaltime = (data.Time == undefined ? 2000 : data.Time);
        if (data.Result != undefined) {
            if (data.Result == 1) {  //操作成功
                $("#ResultMsgDiv").css({ "background-color": "#008800" });
                if (data.Msg == undefined) data.Msg = "操作成功";
            }
            else if (data.Result == 0) {  //操作失敗
                $("#ResultMsgDiv").css({ "background-color": "#FF8800" });
                if (data.Msg == undefined) data.Msg = "操作失敗";
            }
            else if (data.Result == -1) {  //操作失敗
                $("#ResultMsgDiv").css({ "background-color": "#FF8800" });
                if (data.Msg == undefined) data.Msg = "系統異常";
            }

            if (data.Msg != "<null>") {
                $("#ResultMsgDiv").html(data.Msg).fadeIn(totaltime / 3);
                $("#ResultMsgDiv").fadeOut(totaltime * 2 / 3);
            }
            //本窗口跳至某页面
            if (data.Redirect != undefined) {
                if (data.Redirect == "*close") {
                    window.setTimeout("window.parent._close_dialog()", totaltime);
                    return;
                }
                window.setTimeout("window.location.href = '" + data.Redirect + "'", totaltime);
                return;
            }
            //父窗口跳至某页面
            if (data.PRedirect != undefined) {
                window.setTimeout("window.parent.location.href = '" + data.PRedirect + "'", totaltime);
                return;
            }
            //执行相关JS方法
            if (data.fun != undefined) {
                try {
                    var the_fun;
                    eval("the_fun=" + data.fun);
                    the_fun();
                } catch (e) { }
            }
            //刷新主页面
            if (data.Refresh != undefined) {
                window.setTimeout("window.top.location.reload();", totaltime);
            }
        }
    }
    /***操作结果用　弹出层提示　　\\结束***************************************/

    /***弹出操作框　　开始***************************************/
    $("A[openurl]").bind("click",
        function () {
            var openurl = $(this).attr("openurl");
            var opentitle = $(this).attr("opentitle");
            var openproperty = $(this).attr("openproperty");
            if (openproperty == null) {
                _dialog({ src: openurl, title: opentitle, width: 450, height: 400, usebutton: false }).show();
            }
            else {
                var winproperty = { src: openurl, title: opentitle };
                try {
                    //读取自定的属性
                    eval("winproperty=" + openproperty + ";");
                } catch (e) {

                }
                //如果有定openurl则以openurl为准
                if (openurl != null) {
                    winproperty.src = openurl;
                }
                winproperty.src = openurl;
                if (opentitle != null) {
                    winproperty.title = opentitle;
                }
                _dialog(winproperty).show();
            }
        });


    /*src, title, width, height*/
    /*_dialog({src='cc?id=d', 'w'})*/
    window.page_public_dialog = null;
    window._dialog = function (obj) {
        if (obj.scrolling == null || obj.scrolling == undefined)
            obj.scrolling = "auto";
        if ($("#ifr").length == 0) {
            $("body").append("<iframe id=\"ifr\" name=\"ifr\" style=\"display:none;\" scrolling=\"" + obj.scrolling + "\" frameborder=\"0\"></iframe>");
        }
        else {
            $("#ifr").attr("scrolling", obj.scrolling);
        }

        if (obj.title == null)
            obj.title = "系統提示";
        if (obj.width == null)
            obj.width = 850;
        if (obj.height == null)
            obj.height = 600;

        var buttons = null;
        if (obj.usebutton == true) {
            buttons = {
                "确认": function () {
                    if (window.ifr.userConfirm != null) {
                        window.ifr.userConfirm();
                    }
                    $(this).dialog('close');
                },
                "取消": function () {
                    $(this).dialog('close');
                }
            }
        }

        if (obj.position == null) {
            var _result = $("#ifr").attr({ "src": obj.src }).dialog({
                title: obj.title,
                width: obj.width,
                height: obj.height,
                modal: true,
                resizable: false,
                buttons: buttons,
                close: function (event, ui) {
                    $("#ifr").remove();
                }
            });
        }
        else {
            var _result = $("#ifr").attr({ "src": obj.src }).dialog({
                title: obj.title,
                width: obj.width,
                height: obj.height,
                modal: true,
                resizable: false,
                position: obj.position,
                buttons: buttons,
                close: function (event, ui) {
                    $("#ifr").remove();
                }
            });
        }


        $("#ifr").height(parseInt(obj.height, 10) - 20);
        $("#ifr").width(parseInt(obj.width, 10) - 20);
        page_public_dialog = _result;

        return _result;
    }
    window._close_dialog = function () {
        if (page_public_dialog != null) {
            page_public_dialog.dialog("close");
            page_public_dialog = null;
        }
    }
    /***弹出操作框　　\\结束***************************************/
});
