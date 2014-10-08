function doMenu_Hide(ASubObj, imgObj) {
    ASubObj.style.display = 'none';
    imgObj.className = 'closeBlock';
}  // doMenu_Hide

// Раскрывает (отображает) содержимое.
function doMenu_Show(ASubObj, imgObj) {
    ASubObj.style.display = 'block';
    imgObj.className = 'openBlock';
}  // doMenu_Show

// Скрывает или раскрывает содержимое.
function doMenu(AObjIndex) {
    var subObj = document.all['chapter' + AObjIndex];
    var imgObj = document.all['chapter_img' + AObjIndex];
    if (subObj.style.display == 'none') {
        doMenu_Show(subObj, imgObj);
    }
    else {
        doMenu_Hide(subObj, imgObj);
    }  // if..else
}  // doMenu

// Действия при загрузке данной страницы.
function onBodyLoad() {

    doMenu_Hide(document.all['chapter1'], document.all['chapter_img1']);
    doMenu_Hide(document.all['chapter2'], document.all['chapter_img2']);
} 
