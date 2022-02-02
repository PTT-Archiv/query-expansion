export interface SearchEntry {
  entry: string;
  name: string;
  scopeAndContent: string;
  firstMatchingSearchTerm: string;
}

export interface ResponseSummary {
  extendedSearchTerms: Map<string, number>;
  searchEntries: SearchEntry[];
}

export class SearchSettings {
  translatedLocalityNames: boolean;
  historicalLocalityNames: boolean;
}
