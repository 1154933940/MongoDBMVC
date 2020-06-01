//加載更多 moreButtonId 為按鈕
function ScrollLoadMore(moreButtonId, refreshFun) {
    if (typeof moreButtonId == undefined || moreButtonId == "" || moreButtonId == null) {
        return;
    }

    this.moreButtonId = moreButtonId;
    this.refreshFun = refreshFun;
    this.refresh_touch = false;

    //上一次執行時的底端位置
    this.last_bottom_pos = -1;

    //滾動鼠標動作
    $(document).scroll(function () {
        auto_load();
    });

    ////頁面加載時
    $(function () {
        auto_load();
    });

    this.isloading = false;

    //自動加載
    function auto_load() {
        var base = document.ScrollLoadMore_Instance;
        var obj = document.getElementById(base.moreButtonId);
        //按鈕的位置
        var top = getOffsetTop(obj);
        //網頁的高度
        var d_hi = $(document).height();
        //窗口的高度
        var w_hi = $(window).height();
        //滾動條的位置
        var d_scl_top = $(document).scrollTop();

        if (d_scl_top + w_hi > top + obj.clientHeight && top != 0) {
            var bottom_pos = top + obj.clientHeight;
            if (Math.abs(bottom_pos - base.last_bottom_pos) > 10 && !base.isloading) //滑動超過300，這樣一般都超過了半個屏高
            {
                base.isloading = true;
                base.last_bottom_pos = bottom_pos;
                if (obj.tagName.toUpperCase() == "A") {
                    obj.innerHTML = "<img src='http://civetinterface.foxconn.com/Content/images/refresh.gif' style='width: 20px;' align='absmiddle'> 正在拼命加载中..  ";
                }

                //获取点击函数
                $(obj).click(); //obj.click()方法有些手机不支持，改用$(obj).click()

                setTimeout(function () {
                    base.isloading = false;
                    auto_load();
                }, 2000); //两秒钟之后再请求
            }
        }
        if (d_scl_top <= 0 && typeof refreshFun === 'function') {
            base.refresh_touch = true;
            //console.log("base.refresh_touch:" + base.refresh_touch);
            setTimeout(function () {
                if (base.refresh_touch === true) {
                    base.startRefresh();
                    base.refreshFun();
                    //setTimeout(base.endRefresh(), 1000);
                }
            }, 1000);
        }
    }

    $(document).on("touchend", function () {
        var base = document.ScrollLoadMore_Instance;
        base.refresh_touch = false;
        //console.log("base.refresh_touch:" + base.refresh_touch);
    });

    document.ScrollLoadMore_Instance = this;

    //開始刷新
    this.startRefresh = function () {
        //console.log("上拉刷新中..");
        if ($("#srcoll-refrsh-div").length == 0) {
            $("body").append("<div id='srcoll-refrsh-div' "
                + "style='position:fixed; z-index: 999; left: 0px; top: 0px; font-size: 14px; line-height: 35px; width: 100%; text-align: center;'"
                + "><img src='http://civetinterface.foxconn.com/Content/images/refresh.gif' style='width: 20px;' align='absmiddle'> 上拉刷新中</div>");
        }
        $("#srcoll-refrsh-div").show();
    }


    //開始結束
    this.endRefresh = function () {
        $("#srcoll-refrsh-div").fadeOut("slow");
    }
}

function getOffsetTop(DOMObj) {
    var top = DOMObj.offsetTop;
    while (DOMObj = DOMObj.offsetParent) {
        top += DOMObj.offsetTop;
    }
    return top;
}