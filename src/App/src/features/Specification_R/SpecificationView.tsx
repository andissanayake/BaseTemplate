import React, { useEffect, useState } from "react";
import {
  Card,
  Typography,
  Descriptions,
  Button,
  Space,
  notification,
  Spin,
} from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { SpecificationModel } from "./SpecificationModel";

const { Title } = Typography;

const SpecificationView: React.FC = () => {
  const navigate = useNavigate();
  const { specificationId } = useParams<{ specificationId: string }>();
  const [loading, setLoading] = useState(true);
  const [specification, setSpecification] = useState<SpecificationModel | null>(
    null
  );

  useEffect(() => {
    if (specificationId) {
      loadSpecification();
    }
  }, [specificationId]);

  const loadSpecification = () => {
    setLoading(true);
    apiClient.get<SpecificationModel>(
      `/api/specifications/${specificationId}`,
      {
        onSuccess: (data) => {
          setSpecification(data);
        },
        onServerError: () => {
          notification.error({ message: "Failed to load specification!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  };

  if (loading) {
    return (
      <Card>
        <div style={{ textAlign: "center", padding: "50px" }}>
          <Spin size="large" />
        </div>
      </Card>
    );
  }

  if (!specification) {
    return (
      <Card>
        <div style={{ textAlign: "center", padding: "50px" }}>
          <Typography.Text type="secondary">
            Specification not found
          </Typography.Text>
        </div>
      </Card>
    );
  }

  return (
    <Card>
      <div style={{ marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>
          View Specification
        </Title>
      </div>

      <Descriptions bordered column={1}>
        <Descriptions.Item label="Name">{specification.name}</Descriptions.Item>
        <Descriptions.Item label="Description">
          {specification.description}
        </Descriptions.Item>
        <Descriptions.Item label="Parent Specification">
          {specification.parentSpecificationId ? (
            <span>ID: {specification.parentSpecificationId}</span>
          ) : (
            <span style={{ color: "#999" }}>None</span>
          )}
        </Descriptions.Item>
        <Descriptions.Item label="Full Path">
          {specification.fullPath || specification.name}
        </Descriptions.Item>
      </Descriptions>

      <div style={{ marginTop: 16 }}>
        <Space>
          <Button
            type="primary"
            onClick={() => navigate(`/specifications/edit/${specificationId}`)}
          >
            Edit
          </Button>
        </Space>
      </div>
    </Card>
  );
};

export default SpecificationView;
