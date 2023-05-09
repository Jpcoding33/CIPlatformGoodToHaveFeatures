 //js for displaying selected filters

const checkboxes = document.querySelectorAll(".checkbox");

for (var checkbox of checkboxes) {
    checkbox.addEventListener('change', function () {
        if (this.checked == true) {
            addElement(this, this.value);
            // listArray.push(this.value);
            // filterList.innerHTML = listArray.join(' / ');
        }
        else {

            removeElement(this.value);
            // console.log('you unchecked the checkbox');
            // listArray = listArray.filter(e => e !== this.value);
            // filterList.innerHTML = listArray.join(' / ');
            // filterList.removeChild(elementToBeRemoved);
        }
    });
}



function addElement(current, value) {

    const filterList = document.getElementById("selected-filters");
    const node = document.createElement("span");
    node.classList.add('filter-list');
    node.innerHTML = value;
    node.setAttribute('id', value);
    filterList.appendChild(node);
    const crossButton = document.createElement('button');
    crossButton.classList.add('cross-btn');
    const cross = "&times";
    crossButton.innerHTML = cross;
    node.appendChild(crossButton);
    loadMissions(pg = 1);
    crossButton.addEventListener('click', function () {

        const elementToBeRemoved = document.getElementById(value);
        elementToBeRemoved.remove()
        // console.log(elementToBeRemoved);
        current.checked = false;
        loadMissions(pg = 1);
    });

}

function removeElement(value) {

    const filterList = document.getElementById("selected-filters");
    const elementToBeRemoved = document.getElementById(value);
    filterList.removeChild(elementToBeRemoved);
    // console.log(elementToBeRemoved);
    loadMissions(pg = 1);
}