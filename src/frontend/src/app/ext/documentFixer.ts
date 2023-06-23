export class DocumentStyleFixer 
{ 
    constructor(document: Document) {  
        document.body.classList.add('bodyfix');
        document.getElementsByTagName('html')[0].classList.add('htmlfix');
      }
}