import React, { useEffect, useMemo, useState } from 'react';
import { resolveAssetUrl } from '../../api/assets';

type ProductImageGalleryProps = {
  images?: string[];
  reference: string;
};

export const ProductImageGallery = ({ images, reference }: ProductImageGalleryProps) => {
  const [activeIndex, setActiveIndex] = useState(0);
  const [failedImages, setFailedImages] = useState<Set<string>>(new Set());

  const resolvedImages = useMemo(
    () => (images || []).map(resolveAssetUrl).filter((url): url is string => !!url),
    [images]
  );
  const visibleImages = resolvedImages.filter((url) => !failedImages.has(url));

  useEffect(() => {
    if (activeIndex >= visibleImages.length) {
      setActiveIndex(0);
    }
  }, [activeIndex, visibleImages.length]);

  const markImageFailed = (url: string) => {
    setFailedImages((current) => new Set(current).add(url));
  };

  if (visibleImages.length === 0) {
    return (
      <div className="pf-gallery-empty">
        <span className="text-muted fw-medium fs-5">
          <i className="bi bi-image"></i> Image non disponible
        </span>
      </div>
    );
  }

  const mainImage = visibleImages[activeIndex] || visibleImages[0];

  return (
    <div className="row g-3 flex-column-reverse flex-lg-row">
      {visibleImages.length > 1 && (
        <div className="col-lg-2 d-flex flex-row flex-lg-column gap-2 overflow-auto" style={{ maxHeight: '400px' }}>
          {visibleImages.map((img, idx) => (
            <button
              key={img}
              onClick={() => setActiveIndex(idx)}
              className={`pf-gallery-thumb p-0 bg-white rounded overflow-hidden flex-shrink-0 ${
                activeIndex === idx ? 'border border-primary border-2 opacity-100' : 'border border-transparent opacity-50'
              }`}
              style={{ width: '80px', height: '80px', transition: 'all 0.2s' }}
            >
              <img
                src={img}
                alt={`${reference} vue ${idx + 1}`}
                className="w-100 h-100 object-fit-cover"
                onError={() => markImageFailed(img)}
              />
            </button>
          ))}
        </div>
      )}

      <div className="col flex-grow-1">
        <div className="bg-white border rounded overflow-hidden shadow-sm d-flex align-items-center justify-content-center p-3" style={{ height: '400px' }}>
          <img
            src={mainImage}
            alt={`Apercu ${reference}`}
            className="w-100 h-100 object-fit-contain"
            onError={() => markImageFailed(mainImage)}
          />
        </div>
      </div>
    </div>
  );
};
