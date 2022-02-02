import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-search-term',
  templateUrl: './search-term.component.html',
  styleUrls: ['./search-term.component.scss']
})
export class SearchTermComponent {

  @Input()
  searchTerm: string;
  @Input()
  counter: number;
}
