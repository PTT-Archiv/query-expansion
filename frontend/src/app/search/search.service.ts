import {Injectable} from "@angular/core";
import {HttpClient, HttpEvent} from "@angular/common/http";
import {Observable} from "rxjs";
import {ResponseSummary, SearchSettings} from "../model";

@Injectable()
export class SearchService {
  constructor(private http: HttpClient) { }

  baseUrl = 'http://localhost:8080/';

  search(searchTerm: string, translatedLocalityNames: boolean, historicalLocalityNames: boolean): Observable<ResponseSummary> {
    let searchSettings: SearchSettings | null = null;
    if (translatedLocalityNames || historicalLocalityNames) {
      searchSettings = new SearchSettings();
      searchSettings.translatedLocalityNames = translatedLocalityNames;
      searchSettings.historicalLocalityNames = historicalLocalityNames;
    }
    return this.http.post<ResponseSummary>(this.baseUrl + 'search?searchTerm=' + searchTerm, searchSettings);
  }
}
