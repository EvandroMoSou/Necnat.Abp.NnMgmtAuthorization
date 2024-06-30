import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl: 'http://localhost:4200/',
    name: 'NnMgmtAuthorization',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44370/',
    redirectUri: baseUrl,
    clientId: 'NnMgmtAuthorization_App',
    responseType: 'code',
    scope: 'offline_access NnMgmtAuthorization',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44370',
      rootNamespace: 'Necnat.Abp.NnMgmtAuthorization',
    },
    NnMgmtAuthorization: {
      url: 'https://localhost:44311',
      rootNamespace: 'Necnat.Abp.NnMgmtAuthorization',
    },
  },
} as Environment;
