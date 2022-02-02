import {
  AfterContentChecked,
  Directive,
  ElementRef,
  HostBinding,
  Input,
  SecurityContext
} from "@angular/core";
import {DomSanitizer} from "@angular/platform-browser";

@Directive({
  selector: "[highlight]"
})
export class HighlightDirective implements AfterContentChecked {
  @Input("highlight") searchTerm: string;

  @HostBinding("innerHtml")
  content: string | null;

  constructor(private el: ElementRef, private sanitizer: DomSanitizer) {
  }

  ngAfterContentChecked() {
    if (this.el?.nativeElement) {
      const text = (this.el.nativeElement as HTMLElement).textContent;
      if (this.searchTerm === "") {
        this.content = text;
      } else {
        const regex = new RegExp(
          this.searchTerm, "i"
        );
        // @ts-ignore
        const newText = text.replace(regex, (match: string) => {
          return `<mark class="highlight">${match}</mark>`;
        });
        this.content = this.sanitizer.sanitize(
          SecurityContext.HTML,
          newText
        );
      }
     }
  }
}
