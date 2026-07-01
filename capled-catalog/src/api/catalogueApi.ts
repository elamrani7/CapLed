import httpClient from './httpClient';

export const catalogueApi = {
  getCatalogue: async (filters: any = {}) => {
    // Nettoyer les filtres vides
    const params = new URLSearchParams();
    
    Object.keys(filters).forEach(key => {
      const value = filters[key];
      if (value !== null && value !== undefined && value !== '') {
        params.append(key, value);
      }
    });

    return httpClient.get(`/api/v1/catalogue?${params.toString()}`);
  },
  
  getProductById: async (id: number) => {
    return httpClient.get(`/api/v1/catalogue/${id}`);
  },

  getFamilles: async () => {
    return httpClient.get('/api/v1/Famille'); 
  },

  getCategories: async (familleId?: number) => {
    const url = familleId ? `/api/v1/Category?familleId=${familleId}` : '/api/v1/Category';
    return httpClient.get(url);
  }
};
