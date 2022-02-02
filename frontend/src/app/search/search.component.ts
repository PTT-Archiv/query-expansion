import {Component, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {SearchService} from "./search.service";
import {SearchEntry} from "../model";
import {MatTableDataSource} from "@angular/material/table";
import {MatPaginator} from "@angular/material/paginator";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {
  searchForm: FormGroup;
  searchTerms: Map<string, number>;
  searchEntries: SearchEntry[];
  displayedColumns = ['entry', 'name', 'scopeAndContent'];
  loading = false;
  error = false;

  dataSource = new MatTableDataSource<SearchEntry>();
  paginator: MatPaginator;

  @ViewChild(MatPaginator) set matPaginator(matPaginator: MatPaginator) {
    this.paginator = matPaginator;
    this.dataSource.paginator = this.paginator;
  }

  constructor(private formBuilder: FormBuilder, private searchService: SearchService) {
    // initialise search form
    this.searchForm = this.formBuilder.group({
      search: new FormControl(''),
      translatedLocalityNames: new FormControl(false),
      historicalLocalityNames: new FormControl(false),
    });
  }

  onSubmit() {
    this.error = false;
    this.loading = true;
    this.searchService.search(this.searchForm.value.search, this.searchForm.value.translatedLocalityNames, this.searchForm.value.historicalLocalityNames)
      .subscribe((response) => {
      this.searchTerms = response.extendedSearchTerms;
      this.searchEntries = response.searchEntries;
      this.dataSource.data = this.searchEntries;
      this.loading = false;
    }, (error => {
      this.loading = false;
      this.error = true;
    }));
  }
}
