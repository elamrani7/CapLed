import httpClient from './httpClient';

export const leadApi = {
  postLead: (payload: any) => {
    return httpClient.post('/api/leads', payload);
  }
};
