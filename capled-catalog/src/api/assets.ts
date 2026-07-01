import { API_BASE_URL } from './httpClient';

const normalizeAssetPath = (path: string) =>
  path
    .trim()
    .replace(/\\/g, '/')
    .replace(/^\/?wwwroot\//i, '/');

export const resolveAssetUrl = (path?: string | null) => {
  if (!path || !path.trim()) {
    return null;
  }

  const normalized = normalizeAssetPath(path);

  if (/^https?:\/\//i.test(normalized)) {
    return normalized;
  }

  return `${API_BASE_URL}${normalized.startsWith('/') ? normalized : `/${normalized}`}`;
};

export const resolveProductImageUrls = (product?: any) => {
  if (!product) return [];

  const rawPaths = [
    product.image,
    product.urlImagePrincipale,
    ...(Array.isArray(product.images) ? product.images : []),
  ];

  return Array.from(
    new Set(
      rawPaths
        .map(resolveAssetUrl)
        .filter((url): url is string => !!url)
    )
  );
};
