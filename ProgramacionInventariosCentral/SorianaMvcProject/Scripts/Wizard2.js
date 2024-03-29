﻿/* ==========================================================================
   7. How It Works Section
========================================================================== */
.how-it-works {
    background: #f5f5fa;
    padding-bottom: 30px;
}

.board{
    /*width: 75%;
	margin: 60px auto;
	margin-bottom: 0;
	box-shadow: 10px 10px #ccc,-10px 20px #ddd;*/
}
.board .nav-tabs {
    position: relative;
    /* border-bottom: 0; */
    /* width: 80%; */
    margin: 40px auto;
    margin-bottom: 0;
    box-sizing: border-box;

}

.board > div.board-inner > .nav-tabs {
    border: none;
}

p.narrow{
    width: 60%;
    margin: 10px auto;
}

.liner{
    height: 2px;
    background: #ddd;
    position: absolute;
    width: 80%;
    margin: 0 auto;
    left: 0;
    right: 0;
    top: 50%;
    z-index: 1;
}

.nav-tabs > li.active > a, .nav-tabs > li.active > a:hover, .nav-tabs > li.active > a:focus {
    color: #555555;
    cursor: default;
    /* background-color: #ffffff; */
    border: 0;
    border-bottom-color: transparent;
    outline: 0;
}

span.round-tabs{
    width: 70px;
    height: 70px;
    line-height: 70px;
    display: inline-block;
    border-radius: 100px;
    background: white;
    z-index: 2;
    position: absolute;
    left: 0;
    text-align: center;
    font-size: 25px;
}

span.round-tabs.one{
    border: 2px solid #ddd;
    color: #ddd;
}

li.active span.round-tabs.one, li.active span.round-tabs.two, li.active span.round-tabs.three, li.active span.round-tabs.four, li.active span.round-tabs.five {
    background: #69cb95 !important;
    border: 2px solid #69cb95;
    color: #fff;
}

span.round-tabs.two{
    border: 2px solid #ddd;
    color: #ddd;
}

span.round-tabs.three{
    border: 2px solid #ddd;
    color: #ddd;
}

span.round-tabs.four{
    border: 2px solid #ddd;
    color: #ddd;
}

span.round-tabs.five{
    border: 2px solid #ddd;
    color: #ddd;
}

.nav-tabs > li.active > a span.round-tabs{
    background: #fafafa;
}
.nav-tabs > li {
    width: 20%;
}

.nav-tabs > li a{
    width: 70px;
    height: 70px;
    margin: 20px auto;
    border-radius: 100%;
    padding: 0;
}

.nav-tabs > li a:hover{
    background: transparent;
}

.tab-content{
}
.tab-pane{
    position: relative;
    padding-top: 50px;
}

.btn-outline-rounded{
    padding: 10px 40px;
    margin: 20px 0;
    border: 2px solid transparent;
    border-radius: 25px;
}

.btn.green{
    background-color:#69cb95;
    /*border: 2px solid #5cb85c;*/
    color: #ffffff;
}

@media( max-width : 585px ){

    .board {
        width: 90%;
        height:auto !important;
    }
    span.round-tabs {
        font-size:16px;
        width: 50px;
        height: 50px;
        line-height: 50px;
    }
    .tab-content .head{
        font-size:20px;
    }
    .nav-tabs > li a {
        width: 50px;
        height: 50px;
        line-height:50px;
    }

    li.active:after {
        content: " ";
        position: absolute;
        left: 35%;
    }

    .btn-outline-rounded {
        padding:12px 20px;
    }
}